using LiveChartsCore.Kernel;
using LLEAV.Models.FitnessFunction;
using LLEAV.Util;
using LLEAV.ViewModels;
using System;
using System.Collections.Generic;

namespace LLEAV.Models.Algorithms.MIP
{
    public class MIP : ALinkageLearningAlgorithm
    {
        public override string Depiction { get; } = "MIP";

        public override AlgorithmType AlgorithmType { get; } = AlgorithmType.MIP;
        public override bool ShowLocalSearchFunction { get; } = true;
        public override bool ShowGrowthFunction { get; } = true;
        public override bool ShowPopulationSize { get; }

        private MIPHistoryTracker? _tracker;

        public override Tuple<IterationData, IList<IStateChange>> CalculateIteration(IterationData currentIteration, RunData runData)
        {
            _tracker = new MIPHistoryTracker();
            Random random = new Random(currentIteration.RNGSeed);

            IterationData iterationData = currentIteration.Clone();
            iterationData.Iteration = currentIteration.Iteration + 1;

            Dictionary<string, Solution> alreadyInPyramid = new Dictionary<string, Solution>();

            foreach (Population p in iterationData.Populations)
            {
                foreach (Solution s in p.Solutions)
                {
                    alreadyInPyramid.Add(s.Bits.ToString(), s);
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

                if (!alreadyInPyramid.ContainsKey(solutionALS.Bits.ToString()))
                {
                    iterationData.Populations[0].Add(solutionALS);
                    alreadyInPyramid.Add(solutionALS.Bits.ToString(), solutionALS);
                }
            }
            _tracker.IncreaseFitnessEvaluations(solutionsALS);

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

                for (int k = 0; k < solutionsALS.Length; k++)
                {
                    _tracker.SetViewedPopulation(iterationData.Populations[j]);

                    _tracker.ChangeActiveSolution(solutionsALS[k]);

                    Solution y = Crossover(iterationData.Populations[j], iterationData.Populations[j].FOS, runData.FitnessFunction, solutionsALS[k], random);

                    if (y.Fitness > solutionsALS[k].Fitness)
                    {
                        if (!alreadyInPyramid.ContainsKey(y.Bits.ToString()))
                        {
                            inserted = true;
                            if (j == iterationData.Populations.Count - 1)
                            {
                                iterationData.Populations.Add(new Population(iterationData.Populations.Count));
                            }
                            iterationData.Populations[j + 1].Add(y);

                            alreadyInPyramid.Add(y.Bits.ToString(), y);

                            // Tracker
                            populationBeforeFOSUpdate = iterationData.Populations[j + 1].Clone();
                            _tracker.AddedSolutionToNextPopulation(populationBeforeFOSUpdate);
                        }
                    }
                    solutionsALS[k] = y;
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

        public override Population InitialPopulation(RunData runData, Random random)
        {
            return new Population(0);
        }

        private Solution Crossover(Population population, FOS fos,
          AFitnessFunction fitnessFunction, Solution x, Random random)
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
                    _tracker.IncreaseFitnessEvaluations([temp]);
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
