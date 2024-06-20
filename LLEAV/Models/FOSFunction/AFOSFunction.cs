namespace LLEAV.Models.FOSFunction
{
    /// <summary>
    /// Represents an abstract base class for a FOS function.
    /// </summary>
    public abstract class AFOSFunction
    {
        /// <summary>
        /// Gets a string depiction to be shown on the main window.
        /// </summary>
        public abstract string Depiction { get; }
        /// <summary>
        /// Calculates the FOS structure for a given population.
        /// </summary>
        /// <param name="population">The population or which to calculate the FOS.</param>
        /// <param name="numberOfBits">The number of bits a solution has.</param>
        /// <returns>The FOS of the population.</returns>
        public abstract FOS CalculateFOS(Population population, int numberOfBits);
    }
}
