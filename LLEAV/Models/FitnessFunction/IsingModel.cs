using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.FitnessFunction
{
    public class IsingModel : AFitnessFunction
    {
        public override string Depiction { get; } = "Ising Model";

        private const double J = 0;
        private double[] _h;
        private const double MUE = 0;
        public override double Fitness(Solution solution)
        {
            BitList bits = solution.Bits;
            double fitness = 0;

            for (int i = 0; i < bits.NumberBits; i++)
            {
                bool[] values = new bool[2];

                for (int j = 0; j < 2; j++)
                {
                    values[j] = bits.Get((i + j) % bits.NumberBits);
                }

                fitness += f(i, (i+1) % bits.NumberBits, bits);
            }
            return fitness;

        }

        private double f(int i, int j, BitList bits)
        {
            double si = bits.Get(i) ? 1 : -1;
            double sj = bits.Get(j) ? 1 : -1;
            return -J * si * sj - _h[i] * si;
        }

        public override string GetValidationErrorMessage(int solutionLength)
        {
            return "Solution need to be at least " + 2 + " bits long.";
        }

        public override bool ValidateSolutionLength(int solutionLength)
        {
            return solutionLength > 1;
        }

    }
}
