using System;
using System.Collections.Generic;

namespace LLEAV.Models.Algorithms
{
    /// <summary>
    /// Enumeration for the different types of linkage learning algorithms.
    /// </summary>
    public enum AlgorithmType
    {
        GOM,
        ROM,
        MIP
    }

    /// <summary>
    /// Represents an abstract base class for a linkage learning algorithm.
    /// </summary>
    public abstract class ALinkageLearningAlgorithm
    {
        /// <summary>
        /// Gets a string depiction to be shown on the main window.
        /// </summary>
        public abstract string Depiction { get; }

        /// <summary>
        /// Gets the type of the implemented algorithm.
        /// </summary>
        public abstract AlgorithmType AlgorithmType { get; }

        /// <summary>
        /// Gets a value indicating whether the growth function should be shown in the main window.
        /// </summary>
        public abstract bool ShowLocalSearchFunction { get; }
        /// <summary>
        /// Gets a value indicating whether the growth function should be shown in the main window.
        /// </summary>
        public abstract bool ShowGrowthFunction { get; }
        /// <summary>
        /// Gets a value indicating whether the population size should be shown in the main window.
        /// </summary>
        public abstract bool ShowPopulationSize { get; }

        /// <summary>
        /// Calculates the next iteration based on the current iteration data and the run data.
        /// </summary>
        /// <param name="currentIteration">The current iteration data.</param>
        /// <param name="runData">The run data for the algorithm.</param>
        /// <returns>A tuple containing the new iteration data and a list of state changes.</returns>
        public abstract Tuple<IterationData, IList<IStateChange>> CalculateIteration(
            IterationData currentIteration,
            RunData runData);

        /// <summary>
        /// Calculates the state changes for the iteration based on the current iteration data and the run data.
        /// </summary>
        /// <param name="currentIteration">The current iteration data.</param>
        /// <param name="runData">The run data for the algorithm.</param>
        /// <returns>A list of state changes resulting from the iteration.</returns>
        public IList<IStateChange> CalculateIterationStateChanges(
            IterationData currentIteration,
            RunData runData)
        {
            return CalculateIteration(currentIteration, runData).Item2;
        }

        /// <summary>
        /// Initializes the population for the algorithm based on the run data and a random number generator.
        /// </summary>
        /// <param name="runData">The run data for the algorithm.</param>
        /// <param name="random">The random number generator to use for initialization.</param>
        /// <returns>The initial population for the algorithm.</returns>
        public abstract Population InitialPopulation(RunData runData, Random random);
    }
}
