namespace LLEAV.Models.Algorithms
{
    /// <summary>
    /// Interface for visualisation data objects used.
    /// </summary>
    public interface IVisualisationData
    {
        /// <summary>
        /// Clones the visualisation object.
        /// </summary>
        /// <returns>A clone of the visualisation objecct.</returns>
        public abstract IVisualisationData Clone();
    }

    /// <summary>
    /// Interface for state change objects. Only used as a base for algorithm specific state change interfaces.
    /// </summary>
    public interface IStateChange
    {
    }
}
