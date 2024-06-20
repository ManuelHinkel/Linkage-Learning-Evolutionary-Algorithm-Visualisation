using System;

namespace LLEAV.Models.TerminationCriteria
{
    /// <summary>
    /// Represents an abstract base class for termination criteria in an algorithm.
    /// </summary>
    public abstract class ATerminationCriteria
    {
        /// <summary>
        /// Gets a string depiction to be shown on the main window.
        /// </summary>
        public abstract string Depiction { get; }

        /// <summary>
        /// Gets the type of the termiantion argument.
        /// </summary>
        public abstract Type ArgumentType { get; }

        /// <summary>
        /// Determines whether the termination criteria are met for the given iteration data.
        /// </summary>
        /// <param name="iteration">The iteration data to evaluate.</param>
        /// <returns><c>true</c> if the termination criteria are met; otherwise, <c>false</c>.</returns>
        public abstract bool ShouldTerminate(IterationData iteration);

        /// <summary>
        /// Creates an argument for the termination criteria from a string representation.
        /// </summary>
        /// <param name="arg">The string representation of the argument.</param>
        /// <returns><c>true</c> if the argument is created successfully; otherwise, <c>false</c>.</returns>
        public abstract bool CreateArgumentFromString(string arg);

        /// <summary>
        /// Converts the argument to a byte array representation for saving.
        /// </summary>
        /// <returns>A byte array representing the argument.</returns>
        public abstract byte[] ConvertArgumentToBytes();

        /// <summary>
        /// Creates the argument for the termination criteria from a byte array representation.
        /// </summary>
        /// <param name="bytes">The byte array representation of the argument.</param>
        /// <returns><c>true</c> if the argument is created successfully; otherwise, <c>false</c>.</returns>
        public abstract bool CreateArgumentFromBytes(byte[] bytes);

        /// <summary>
        /// Created the string, which should be shown when the termination criteria is met.
        /// </summary>
        /// <returns>The string shown when the termination criteria is met.</returns>
        public abstract string GetTerminationString();
    }
}
