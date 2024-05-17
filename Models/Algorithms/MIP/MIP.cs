using LiveChartsCore.Kernel;
using LLEAV.Models.FitnessFunction;
using LLEAV.Util;
using LLEAV.ViewModels;
using System;
using System.Collections.Generic;

namespace LLEAV.Models.Algorithms.MIP
{
    public class MIP : ILinkageLearningAlgorithm
    {
        private MIPHistoryTracker? _tracker;

        public Tuple<IterationData, IList<IStateChange>> CalculateIteration(IterationData currentIteration, RunData runData)
        {
            _tracker = new MIPHistoryTracker();
            Random random = new Random(currentIteration.RNGSeed);

            IterationData iterationData = currentIteration.Clone();
            iterationData.Iteration = currentIteration.Iteration + 1;

            Dictionary<long, Solution> alreadyInPyramid = new Dictionary<long, Solution>();

            foreach (Population p in iterationData.Populations)
            {
                foreach (Solution s in p.Solutions)
                {
                    alreadyInPyramid.Add(s.GetHashCode(), s);
                }
            }
            _tracker.SetViewedPopulation(iterationData.Populations[0].Clone());

            int numberOfNewSolutions = runData.GrowthFunction.GetNumberOfNewSolutions(iterationData.Iteration);

            Solution[] solutions = new Solution[numberOfNewSolutions];
            Solution[] solutionsALS = new Solution[numberOfNewSolutions];
            List<Tuple<Solution, Solution>> generated = new List<Tuple<Solution, Solution>>();
            for (int i = 0; i < numberOfNewSolutions; i++)
            {
                solutions[i] = Solution.RandomSolution(runData.NumberOfBits, random);
                solutionsALS[i] = runData.LocalSearchFunction.Execute(solutions[i], runData.FitnessFunction, random);
                generated.Add(new Tuple<Solution, Solution>(solutions[i], solutionsALS[i]));
            }

            _tracker.SetGeneratedSolutions(generated);
         
            // Add newly generated solutions to lowest level
            foreach(Solution solutionALS in solutionsALS)
            {
                // Not guaranteed that localsearch function sets the fitness
                solutionALS.Fitness = runData.FitnessFunction.Fitness(solutionALS);

                if (!alreadyInPyramid.ContainsKey(solutionALS.GetHashCode()))
                {
                    iterationData.Populations[0].Add(solutionALS);
                    alreadyInPyramid.Add(solutionALS.GetHashCode(), solutionALS);
                }
            }

            // Tracker
            var populationBeforeFOSUpdate = iterationData.Populations[0].Clone();
            _tracker.AddedSolutionToNextPopulation(populationBeforeFOSUpdate);

            iterationData.Populations[0].FOS
                = runData.FOSFunction.CalculateFOS(iterationData.Populations[0], runData.NumberOfBits);

            // Tracker
            var populationAfterFOSUpdate = iterationData.Populations[0].Clone();
            populationAfterFOSUpdate.Previous = populationBeforeFOSUpdate;
            _tracker.UpdateFOS(populationAfterFOSUpdate);


            int j = 0;
            while (j < iterationData.Populations.Count)
            {
                _tracker.ChangeDonors(iterationData.Populations[j]);

                bool inserted = false;

                foreach (Solution solutionALS in solutionsALS)
                {
                    _tracker.SetViewedPopulation(iterationData.Populations[j]);
                   
                    _tracker.ChangeActiveSolution(solutionALS);

                    Solution y = Crossover(iterationData.Populations[j], iterationData.Populations[j].FOS, runData.FitnessFunction, solutionALS, random);
                    y.Fitness = runData.FitnessFunction.Fitness(y);

                    if (y.Fitness > solutionALS.Fitness)
                    {
                        if (!alreadyInPyramid.ContainsKey(y.GetHashCode()))
                        {
                            inserted = true;
                            if (j == iterationData.Populations.Count - 1)
                            {
                                iterationData.Populations.Add(new Population(iterationData.Populations.Count));
                            }
                            iterationData.Populations[j + 1].Add(y);

                            alreadyInPyramid.Add(y.GetHashCode(), y);

                            // Tracker
                            populationBeforeFOSUpdate = iterationData.Populations[j + 1].Clone();
                            _tracker.AddedSolutionToNextPopulation(populationBeforeFOSUpdate);
                        }
                    }
                }

                if (inserted)
                {

                    iterationData.Populations[j + 1].FOS = runData.FOSFunction.CalculateFOS(iterationData.Populations[j + 1], runData.NumberOfBits);

                    // Tracker
                    populationAfterFOSUpdate = iterationData.Populations[j + 1].Clone();
                    populationAfterFOSUpdate.Previous = populationBeforeFOSUpdate;
                    _tracker.UpdateFOS(populationAfterFOSUpdate);
                }

                j++;
            }


            iterationData.LastIteration = runData.TerminationCriteria.ShouldTerminate(iterationData);
            iterationData.RNGSeed = random.Next();

            _tracker.SetTermination(iterationData.LastIteration);

            return new Tuple<IterationData, IList<IStateChange>>(iterationData, _tracker.StateChangeHistory);
        }

        public AlgorithmType GetAlgorithmType()
        {
            return AlgorithmType.MIP;
        }

        public Population InitialPopulation(RunData runData, Random random)
        {
            return new Population(0);
        }

        private Solution Crossover(Population population, FOS fos,
          IFitnessFunction fitnessFunction, Solution x, Random random)
        {
            foreach (Cluster c in fos)
            {
                _tracker.ChangeFOSCluster(c);

                List<Solution> shuffled = new List<Solution>(population);
                shuffled.Shuffle(random);


                foreach (Solution donor in shuffled)
                {
                    _tracker.ChangeDonor(donor);

                    Solution temp = Solution.Merge(x, donor, c);
                    _tracker.Merge(temp);

                    temp.Fitness = fitnessFunction.Fitness(temp);
                    if (temp.Fitness > x.Fitness)
                    {
                        x = temp;
                        _tracker.ApplyCrossover(temp);
                        break;
                    }
                }
            }
            return x;
        }
    }
}
