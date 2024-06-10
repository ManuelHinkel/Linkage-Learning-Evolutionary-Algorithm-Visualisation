namespace LLEAV.Models.FitnessFunction
{
    public abstract class AFitnessFunction
    {
        public abstract string Depiction { get; }
        public abstract bool EnableArg { get; }

        public abstract double Fitness(Solution solution);

        public abstract bool ValidateSolutionLength(int solutionLength);

        public abstract string GetSolutionLengthValidationErrorMessage(int solutionLength);


        public abstract bool CreateArgumentFromString(string arg);
        public abstract string GetArgValidationErrorMessage(string arg);
        public abstract byte[] ConvertArgumentToBytes();
        public abstract bool CreateArgumentFromBytes(byte[] bytes);

    }

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
