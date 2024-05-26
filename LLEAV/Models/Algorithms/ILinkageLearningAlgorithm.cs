﻿using LLEAV.ViewModels;
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

    public interface ILinkageLearningAlgorithm
    {
        public Tuple<IterationData, IList<IStateChange>> CalculateIteration(
            IterationData currentIteration,
            RunData runData);

        public IList<IStateChange> CalculateIterationStateChanges(
            IterationData currentIteration,
            RunData runData)
        {
            return CalculateIteration(currentIteration, runData).Item2;
        }

        public AlgorithmType GetAlgorithmType();

        public Population InitialPopulation(RunData runData, Random random);
    }
}