using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.FitnessFunction
{
    public class TestFitness : IFitnessFunction
    {
        public double Fitness(Solution solution)
        {
            double value = 0;
            value += solution.Bits.Get(0) & solution.Bits.Get(1) ? 1 : 0;
            value += solution.Bits.Get(2) ^ solution.Bits.Get(3) ? 1 : 0;
            value += 3 * (solution.Bits.Get(4) ^ solution.Bits.Get(5) ^ solution.Bits.Get(6) ? 1 : 0);
            value += 2 * (solution.Bits.Get(3) & solution.Bits.Get(4) ? 1 : 0);
            return value;
        }

        public string GetValidationErrorMessage(int solutionLength)
        {
            return "Solution length must be at least seven.";
        }

        public bool ValidateSolutionLength(int solutionLength)
        {
            return solutionLength > 6;
        }
    }
}
