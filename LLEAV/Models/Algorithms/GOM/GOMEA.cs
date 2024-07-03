using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLEAV.Models.Algorithms;
using LLEAV.Models.FitnessFunction;
using LLEAV.Util;
using LLEAV.ViewModels;

namespace LLEAV.Models.Algorithms.GOM
{
    public class GOMEA : ALinkageLearningAlgorithm
    {
        public override string Depiction { get; } = "GOMEA";

        public override AlgorithmType AlgorithmType { get; } = AlgorithmType.GOM;
        public override bool ShowLocalSearchFunction { get; }
        public override bool ShowGrowthFunction { get; }
        public override bool ShowPopulationSize { get; } = true;

        private GOMEAHistoryTracker? _tracker;

        public override Tuple<IterationData, IList<IStateChange>> CalculateIteration(IterationData currentIteration, RunData runData)
        {
            _tracker = new GOMEAHistoryTracker();
            Random random = new Random(currentIteration.RNGSeed);

            IterationData iterationData = currentIteration.Clone();

            Population population = iterationData.Populations[0];

            _tracker.SetViewedPopulation(population.Clone());

            IList<Solution> newSolutions = new List<Solution>();

            for(int i = 0; i < runData.NumberOfSolutions; i++)
            {
                _tracker.ChangeActiveSolution(population.Solutions[i]);
                newSolutions.Add(GOM(population.Solutions[i], population, population.FOS, runData.FitnessFunction, random));
                _tracker.AddSolutionToNextIteration(newSolutions[i]);
            }

            population.ClearAndAddAll(
                AlgorithmFunctions.TournamentSelection(newSolutions, runData.NumberOfSolutions, 2, random)
            );
            _tracker.ApplyTournamentSelection(population.Solutions);

            population.FOS
                = runData.FOSFunction.CalculateFOS(population, runData.NumberOfBits);

            _tracker.UpdateFOS(population.Clone());

            iterationData.Iteration = currentIteration.Iteration + 1;
            iterationData.LastIteration = runData.TerminationCriteria.ShouldTerminate(iterationData);
            iterationData.RNGSeed = random.Next();

            _tracker.SetTermination(iterationData.LastIteration);

            return new Tuple<IterationData, IList<IStateChange>>(iterationData, _tracker.StateChangeHistory);
        }

        
       
        private Solution GOM(Solution toMix, Population population, FOS fos, AFitnessFunction fitnessFunction, Random random)
        {
            Solution o = toMix.Clone();

            foreach (Cluster c in fos)
            {
                _tracker.ChangeFOSCluster(c);
                Solution parent = population.Solutions[random.Next(0, population.Solutions.Count)];
                _tracker.ChangeDonor(parent);

                Solution temp = Solution.Merge(o, parent, c);

                _tracker.Merge(temp);

                if (!(temp.Bits & c.Mask).Equals(o.Bits & c.Mask))
                {
                    temp.Fitness = fitnessFunction.Fitness(temp);
                    _tracker.IncreaseFitnessEvaluations([temp]);
                    if (temp.Fitness > o.Fitness)
                    {
                        o = temp;
                        _tracker.ApplyCrossover(temp);
                    }
                }
            }

            return o;
        }

        public override Population InitialPopulation(RunData runData, Random random)
        {
            Population initial = new Population(0);

            for (int i = 0; i < runData.NumberOfSolutions; i++)
            {
                var s = Solution.RandomSolution(runData.NumberOfBits, random);
                s.Fitness = runData.FitnessFunction.Fitness(s);
                initial.Add(s);
            }

            initial.ClearAndAddAll(AlgorithmFunctions.TournamentSelection(initial.Solutions, runData.NumberOfSolutions, 2, random));
            
            foreach( Solution s in initial)
            {
                s.Fitness = runData.FitnessFunction.Fitness(s);
            }

            initial.FOS = runData.FOSFunction.CalculateFOS(initial, runData.NumberOfBits);

            return initial;
        }
    }
}
