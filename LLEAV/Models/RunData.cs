using LLEAV.Models.Algorithms;
using LLEAV.Models.FitnessFunction;
using LLEAV.Models.FOSFunction;
using LLEAV.Models.GrowthFunction;
using LLEAV.Models.LocalSearchFunction;
using LLEAV.Models.TerminationCriteria;
using System.Collections.Generic;
using System.Linq;

namespace LLEAV.Models
{
    /// <summary>
    /// Represents data for a single iteration in an evolutionary algorithm run.
    /// </summary>
    public class IterationData
    {
        /// <summary>
        /// Gets or sets the list of populations in the current iteration.
        /// </summary>
        public IList<Population> Populations { get; set; }

        /// <summary>
        /// Gets or sets the iteration number.
        /// </summary>
        public int Iteration { get; set; } = -1;

        /// <summary>
        /// Gets or sets a value indicating whether this is the last iteration.
        /// </summary>
        public bool LastIteration { get; set; }

        /// <summary>
        /// Gets or sets the random number generator seed used in this iteration.
        /// </summary>
        public int RNGSeed { get; set; }

        private IterationData() { }

        /// <summary>
        /// Initializes a new instance of the iteration data with the specified initial population and RNG seed.
        /// </summary>
        /// <param name="initialPopulation">The initial population.</param>
        /// <param name="rngSeed">The RNG seed used for this iteration.</param>
        public IterationData(Population initialPopulation, int rngSeed)
        {
            Populations = [initialPopulation];
            RNGSeed = rngSeed;
        }

        /// <summary>
        /// Creates a deep copy of the current iteration data.
        /// </summary>
        /// <returns>A new instance of <see cref="IterationData"/> that is a copy of the current instance.</returns>
        public IterationData Clone()
        {
            IterationData clone = new IterationData()
            {
                Iteration = this.Iteration,
                LastIteration = this.LastIteration,
                RNGSeed = this.RNGSeed,
                Populations = new List<Population>(Populations.Select(p => p.Clone())),
            };

            return clone;
        }
    }

    /// <summary>
    /// Represents the data for a complete run of an evolutionary algorithm.
    /// </summary>
    public class RunData
    {
        /// <summary>
        /// Gets or sets the list of iteration data for the run.
        /// </summary>
        public IList<IterationData> Iterations { get; set; } = new List<IterationData>();

        /// <summary>
        /// Gets or sets the algorithm used in the run.
        /// </summary>
        public ALinkageLearningAlgorithm Algorithm { get; set; }

        /// <summary>
        /// Gets or sets the fitness function used in the run.
        /// </summary>
        public AFitnessFunction FitnessFunction { get; set; }

        /// <summary>
        /// Gets or sets the local search function used in the run.
        /// </summary>
        public ALocalSearchFunction LocalSearchFunction { get; set; }

        /// <summary>
        /// Gets or sets the FOS function used in the run.
        /// </summary>
        public AFOSFunction FOSFunction { get; set; }

        /// <summary>
        /// Gets or sets the termination criteria used in the run.
        /// </summary>
        public ATerminationCriteria TerminationCriteria { get; set; }

        // <summary>
        /// Gets or sets the growth function used in the run.
        /// </summary>
        public AGrowthFunction GrowthFunction { get; set; }

        /// <summary>
        /// Gets or sets the number of bits used in the solutions.
        /// </summary>
        public int NumberOfBits { get; set; }

        /// <summary>
        /// Gets or sets the number of solutions in each population.
        /// </summary>
        public int NumberOfSolutions { get; set; }

        /// <summary>
        /// Gets or sets the random number generator seed used for the run.
        /// </summary>
        public int RNGSeed { get; set; }

        /// <summary>
        /// Gets or sets the file path where the run data is stored.
        /// </summary>
        public string? FilePath { get; set; }

    }
}
