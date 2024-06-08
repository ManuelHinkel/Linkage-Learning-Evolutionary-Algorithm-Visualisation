using LLEAV.ViewModels;
using System;
using System.Collections.Generic;

namespace LLEAV.Models.Algorithms
{
    public enum AlgorithmType
    {
        GOM,
        ROM,
        MIP
    }

    public abstract class ALinkageLearningAlgorithm
    {
        public abstract string Depiction { get; }
        public abstract AlgorithmType AlgorithmType { get; }

        public abstract bool ShowLocalSearchFunction { get; }
        public abstract bool ShowGrowthFunction { get; }
        public abstract bool ShowPopulationSize { get; }


        public abstract Tuple<IterationData, IList<IStateChange>> CalculateIteration(
            IterationData currentIteration,
            RunData runData);

        public IList<IStateChange> CalculateIterationStateChanges(
            IterationData currentIteration,
            RunData runData)
        {
            return CalculateIteration(currentIteration, runData).Item2;
        }


        public abstract Population InitialPopulation(RunData runData, Random random);
    }
}
