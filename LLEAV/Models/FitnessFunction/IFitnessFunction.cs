namespace LLEAV.Models.FitnessFunction
{
    public interface IFitnessFunction
    {
        public double Fitness(Solution solution);

        public bool ValidateSolutionLength(int solutionLength);

        public string GetValidationErrorMessage(int solutionLength);
    }
}
