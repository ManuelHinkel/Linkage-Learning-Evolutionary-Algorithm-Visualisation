using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Painting;
using LLEAV.Models.FitnessFunction;
using LLEAV.Util;
using LLEAV.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LLEAV.Models.Algorithms.ROM
{
    public class ROMEA : ALinkageLearningAlgorithm
    {
        public override string Depiction { get; } = "ROMEA";

        public override AlgorithmType AlgorithmType { get; } = AlgorithmType.ROM;

        public override bool ShowLocalSearchFunction { get; }
        public override bool ShowGrowthFunction { get; }
        public override bool ShowPopulationSize { get; } = true;

        private ROMEAHistoryTracker? _tracker;

        public override Tuple<IterationData, IList<IStateChange>> CalculateIteration(IterationData currentIteration, RunData runData)
        {
            _tracker = new ROMEAHistoryTracker();
            Random random = new Random(currentIteration.RNGSeed);

            IterationData iterationData = currentIteration.Clone();

            Population population = iterationData.Populations[0];

            _tracker.SetViewedPopulation(population.Clone());

            IList<Solution> newSolutions = new List<Solution>();

            for (int i = 0; i < runData.NumberOfSolutions; i++)
            {
                newSolutions.Add(ROM(population.Solutions[i], population, population.FOS, runData.FitnessFunction, random));
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



        private Solution ROM(Solution toMix, Population population, FOS fos, AFitnessFunction fitnessFunction, Random random)
        {
            Solution o0 = toMix.Clone();
            Solution p0 = toMix.Clone();

            Solution r = population.Solutions[random.Next(0, population.Solutions.Count)];

            Solution o1 = r.Clone();
            Solution p1 = r.Clone();

            _tracker.ChangeActiveSolutions(o0, o1, p0, p1);

            foreach (Cluster c in fos)
            {
                _tracker.ChangeFOSCluster(c);
                o0 = Solution.Merge(o0, p1, c);
                o1 = Solution.Merge(o1, p0, c);

                _tracker.Crossover(o0.Clone(), o1.Clone());

                if (!(o0.Bits & c.Mask).Equals(o1.Bits & c.Mask))
                {
                    o0.Fitness = fitnessFunction.Fitness(o0);

                    if (o0.Fitness > p0.Fitness)
                    {
                        p0 = Solution.Merge(p0, o0, c);
                        p0.Fitness = o0.Fitness;

                        p1 = Solution.Merge(p1, o1, c);

                        _tracker.FitnessIncrease(p0.Clone(), p1.Clone());
                    } else
                    {
                        o0 = Solution.Merge(o0, p0, c);
                        o0.Fitness = p0.Fitness;

                        o1 = Solution.Merge(o1, p1, c);
                        _tracker.FitnessDecrease(o0.Clone(), o1.Clone());
                    }
                } 
            }

            return o0;
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

            foreach (Solution s in initial)
            {
                s.Fitness = runData.FitnessFunction.Fitness(s);
            }

            initial.FOS = runData.FOSFunction.CalculateFOS(initial, runData.NumberOfBits);

            return initial;
        }
    }
}
