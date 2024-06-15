using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.FitnessFunction
{
    public class OneMax: AFitnessFunctionNoArg
    {
        public override string Depiction { get; } = "One Max";

        public override double Fitness(Solution solution)
        {
            int ones = 0;
            for (int i = 0; i < solution.Bits.NumberBits; i++)
            {
                if (solution.Bits.Get(i))
                {
                    ones++;
                }
            }
            return ones;
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
