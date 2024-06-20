using LLEAV.Models.FitnessFunction;
using System;

namespace LLEAV.Models.LocalSearchFunction
{
    /// <summary>
    /// Represents an abstract base class for a local search function.
    /// </summary>
    public abstract class ALocalSearchFunction
    {
        /// <summary>
        /// Gets a string depiction to be shown on the main window.
        /// </summary>
        public abstract string Depiction { get; }

        /// <summary>
        /// Executes a local search function on a given solution, a fitness function and uses the given random number generator for 
        /// randomised operations.
        /// </summary>
        /// <param name="solution">The solution to execute local search on.</param>
        /// <param name="fitnessFunction">The fitness function used in the local search.</param>
        /// <param name="random">The random number generator for randomised operations.</param>
        /// <returns>The solution after local search was executed.</returns>
        public abstract Solution Execute(Solution solution, AFitnessFunction fitnessFunction, Random random);
    }
}
