using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.FitnessFunction
{
    public class OneMax : IFitnessFunction
    {
        public double Fitness(Solution solution)
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

        public string GetValidationErrorMessage(int solutionLength)
        {
            return "Solution length must be at least one.";
        }

        public bool ValidateSolutionLength(int solutionLength)
        {
            return solutionLength > 0;
        }
    }
}
