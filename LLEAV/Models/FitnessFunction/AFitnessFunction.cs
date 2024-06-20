namespace LLEAV.Models.FitnessFunction
{
    /// <summary>
    /// Represents an abstract base class for afitness function.
    /// </summary>
    public abstract class AFitnessFunction
    {
        /// <summary>
        /// Gets a string depiction to be shown on the main window.
        /// </summary>
        public abstract string Depiction { get; }
        /// <summary>
        /// Gets a value indicating whether argument inputs are enabled for the fitness function.
        /// </summary>
        public abstract bool EnableArg { get; }

        /// <summary>
        /// Calculates the fitness of the given solution.
        /// </summary>
        /// <param name="solution">The solution for which to calculate fitness.</param>
        /// <returns>The fitness value of the solution.</returns>
        public abstract double Fitness(Solution solution);

        /// <summary>
        /// Validates the length of the given solution.
        /// </summary>
        /// <param name="solutionLength">The length of the solution to validate.</param>
        /// <returns><c>true</c> if the solution length is valid; otherwise, <c>false</c>.</returns>
        public abstract bool ValidateSolutionLength(int solutionLength);

        /// <summary>
        /// Gets the validation error message for an invalid solution length.
        /// </summary>
        /// <param name="solutionLength">The length of the solution that is invalid.</param>
        /// <returns>A string containing the validation error message.</returns>
        public abstract string GetSolutionLengthValidationErrorMessage(int solutionLength);

        /// <summary>
        /// Creates the argument for the fitness function from a string representation.
        /// </summary>
        /// <param name="arg">The string representation of the argument.</param>
        /// <returns><c>true</c> if the argument is created successfully; otherwise, <c>false</c>.</returns>
        public abstract bool CreateArgumentFromString(string arg);

        /// <summary>
        /// Gets the validation error message for an invalid argument.
        /// </summary>
        /// <param name="arg">The argument that is invalid.</param>
        /// <returns>A string containing the validation error message.</returns>
        public abstract string GetArgValidationErrorMessage(string arg);

        /// <summary>
        /// Converts the argument to a byte array representation for saving.
        /// </summary>
        /// <returns>A byte array representing the argument.</returns>
        public abstract byte[] ConvertArgumentToBytes();

        /// <summary>
        /// Creates an argument for the fitness function from a byte array representation.
        /// </summary>
        /// <param name="bytes">The byte array representation of the argument.</param>
        /// <returns><c>true</c> if the argument is created successfully; otherwise, <c>false</c>.</returns>
        public abstract bool CreateArgumentFromBytes(byte[] bytes);

    }

    /// <summary>
    /// Represents an abstract base class for a fitness function with no arguments.
    /// </summary>
    public abstract class AFitnessFunctionNoArg : AFitnessFunction
    {
        public override bool EnableArg => false;

        public override byte[] ConvertArgumentToBytes()
        {
            return [];
        }

        public override bool CreateArgumentFromBytes(byte[] bytes)
        {
            return true;
        }

        public override bool CreateArgumentFromString(string arg)
        {
            return true;
        }

        public override string GetArgValidationErrorMessage(string arg)
        {
            return "";
        }
    }
}
