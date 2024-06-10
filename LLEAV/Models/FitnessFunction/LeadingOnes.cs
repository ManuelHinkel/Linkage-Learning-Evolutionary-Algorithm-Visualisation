using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.FitnessFunction
{
    public class LeadingOnes : AFitnessFunctionNoArg
    {
        public override string Depiction { get; } = "Leading Ones";

        public override double Fitness(Solution solution)
        {
            int leadingOnes = 0;
            for (int i = 0; i < solution.Bits.NumberBits; i++)
            {
                if (solution.Bits.Get(i))
                {
                    leadingOnes++;
                }
                else
                {
                    break;
                }
            }
            return leadingOnes;
        }

        public override string GetSolutionLengthValidationErrorMessage(int solutionLength)
        {
            return "Solution length must be at least one.";
        }

        public override bool ValidateSolutionLength(int solutionLength)
        {
            return solutionLength > 0;
        }
    }
}
