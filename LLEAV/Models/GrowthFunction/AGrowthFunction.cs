namespace LLEAV.Models.GrowthFunction
{
    /// <summary>
    /// Represents an abstract base class for a growth function.
    /// </summary>
    public abstract class AGrowthFunction
    {
        /// <summary>
        /// Gets a string depiction to be shown on the main window.
        /// </summary>
        public abstract string Depiction { get; }

        /// <summary>
        /// Calculates the number of solutions that should be created for a given iteration.
        /// </summary>
        /// <param name="iteration">The iteration for which to calculate the number of solutions.</param>
        /// <returns>The number of solutions that should be created for a given iteration.</returns>
        public abstract int GetNumberOfNewSolutions(int iteration);
    }
}
