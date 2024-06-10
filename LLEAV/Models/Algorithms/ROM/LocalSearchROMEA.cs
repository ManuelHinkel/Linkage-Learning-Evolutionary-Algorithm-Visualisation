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
    public class LocalSearchROMEA : ALinkageLearningAlgorithm
    {
        public override string Depiction { get; } = "ROMEA (Local Search)";

        public override AlgorithmType AlgorithmType { get; } = AlgorithmType.ROM;

        public override bool ShowLocalSearchFunction { get; } = true;
        public override bool ShowGrowthFunction { get; }
        public override bool ShowPopulationSize { get; } = true;

        private ROMEA _romea = new ROMEA();

        public override Tuple<IterationData, IList<IStateChange>> CalculateIteration(IterationData currentIteration, RunData runData)
        {
            return _romea.CalculateIteration(currentIteration, runData);
        }

        public override Population InitialPopulation(RunData runData, Random random)
        {
            Population initial = new Population(0);

            for (int i = 0; i < runData.NumberOfSolutions; i++)
            {
                var s = Solution.RandomSolution(runData.NumberOfBits, random);
                s = runData.LocalSearchFunction.Execute(s, runData.FitnessFunction, random);
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
