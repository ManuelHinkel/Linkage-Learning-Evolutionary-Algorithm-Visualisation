using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.FitnessFunction
{
    public class IsingRing : AFitnessFunctionNoArg
    {
        public override string Depiction { get; } = "Ising Ring";

        public override double Fitness(Solution solution)
        {
            BitList bits = solution.Bits;
            double fitness = 0;

            for (int i = 0; i < bits.NumberBits; i++)
            {
                fitness += bits.Get(i) == bits.Get((i + 1) % bits.NumberBits) ? 1 : 0;
            }
            return fitness;

        }

        public override string GetSolutionLengthValidationErrorMessage(int solutionLength)
        {
            return "Solution need to be at least " + 2 + " bits long.";
        }

        public override bool ValidateSolutionLength(int solutionLength)
        {
            return solutionLength > 1;
        }
    }
}
