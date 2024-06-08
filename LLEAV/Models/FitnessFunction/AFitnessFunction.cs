namespace LLEAV.Models.FitnessFunction
{
    public abstract class AFitnessFunction
    {
        public abstract string Depiction { get; }

        public abstract double Fitness(Solution solution);

        public abstract bool ValidateSolutionLength(int solutionLength);

        public abstract string GetValidationErrorMessage(int solutionLength);

    }
}
