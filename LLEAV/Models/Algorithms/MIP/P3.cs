using LLEAV.Models.FitnessFunction;
using LLEAV.Models.FOSFunction;
using LLEAV.Models.LocalSearchFunction;
using LLEAV.Util;
using LLEAV.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LLEAV.Models.Algorithms.MIP
{
    public class P3 : ALinkageLearningAlgorithm
    {
        public override string Depiction { get; } = "P3";

        public override AlgorithmType AlgorithmType { get; } = AlgorithmType.MIP;

        public override bool ShowLocalSearchFunction { get; } = true;
        public override bool ShowGrowthFunction { get; }
        public override bool ShowPopulationSize { get; }

        private MIPHistoryTracker? _tracker;

        public override Tuple<IterationData, IList<IStateChange>> CalculateIteration(IterationData currentIteration, RunData runData)
        {
            _tracker = new MIPHistoryTracker();
            Random random = new Random(currentIteration.RNGSeed);

            IterationData iterationData = currentIteration.Clone();
            Dictionary<long, Solution> alreadyInPyramid = new Dictionary<long, Solution>();

            foreach (Population p in iterationData.Populations) 
            { 
                foreach (Solution s in p.Solutions) 
                {
                    alreadyInPyramid.Add(s.GetHashCode(), s);
                }
            }
            _tracker.SetViewedPopulation(iterationData.Populations[0].Clone());

            // Create new solution and improve it by local search
            Solution solution = Solution.RandomSolution(runData.NumberOfBits, random);
            Solution solutionALS = runData.LocalSearchFunction.Execute(solution, runData.FitnessFunction, random);

            _tracker.SetGeneratedSolutions([new Tuple<Solution, Solution>(solution, solutionALS)]);

            // Not guaranteed that localsearch function sets the fitness
            solutionALS.Fitness = runData.FitnessFunction.Fitness(solutionALS);

            if (!alreadyInPyramid.ContainsKey(solutionALS.GetHashCode()))
            {
                iterationData.Populations[0].Add(solutionALS);
                alreadyInPyramid.Add(solutionALS.GetHashCode(), solutionALS);

                // Tracker
                var populationBeforeFOSUpdate = iterationData.Populations[0].Clone();
                _tracker.AddedSolutionToNextPopulation(populationBeforeFOSUpdate);

                iterationData.Populations[0].FOS 
                    = runData.FOSFunction.CalculateFOS(iterationData.Populations[0], runData.NumberOfBits);

                // Tracker
                var populationAfterFOSUpdate = iterationData.Populations[0].Clone();
                populationAfterFOSUpdate.Previous = populationBeforeFOSUpdate;
                _tracker.UpdateFOS(populationAfterFOSUpdate);
            }


            int max = iterationData.Populations.Count;
            for (int j = 0; j < max; j++)
            {
                _tracker.ChangeDonors(iterationData.Populations[j]);
                _tracker.ChangeActiveSolution(solutionALS);

                Solution y = Crossover(iterationData.Populations[j], iterationData.Populations[j].FOS, runData.FitnessFunction, solutionALS, random);
                y.Fitness = runData.FitnessFunction.Fitness(y);

                if (y.Fitness > solutionALS.Fitness)
                {
                    if (!alreadyInPyramid.ContainsKey(y.GetHashCode()))
                    {
                        if (j == max - 1)
                        {
                            iterationData.Populations.Add(new Population(iterationData.Populations.Count));
                        }
                        iterationData.Populations[j+1].Add(y);

                        // Tracker
                        var populationBeforeFOSUpdate = iterationData.Populations[j+1].Clone();
                        _tracker.AddedSolutionToNextPopulation(populationBeforeFOSUpdate);


                        alreadyInPyramid.Add(y.GetHashCode(), y);
                        iterationData.Populations[j + 1].FOS = runData.FOSFunction.CalculateFOS(iterationData.Populations[j + 1], runData.NumberOfBits);

                        // Tracker
                        var populationAfterFOSUpdate = iterationData.Populations[j+1].Clone();
                        populationAfterFOSUpdate.Previous = populationBeforeFOSUpdate;
                        _tracker.UpdateFOS(populationAfterFOSUpdate);
                    }
                }
            }


            iterationData.Iteration = currentIteration.Iteration + 1;
            iterationData.LastIteration = runData.TerminationCriteria.ShouldTerminate(iterationData);
            iterationData.RNGSeed = random.Next();

            _tracker.SetTermination(iterationData.LastIteration);
            
            return new Tuple<IterationData, IList<IStateChange>>(iterationData, _tracker.StateChangeHistory);
        }

        private Solution Crossover(Population population,FOS fos,
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

        public override Population InitialPopulation(RunData runData, Random random)
        {
            return new Population(0);
        }
    }
}
