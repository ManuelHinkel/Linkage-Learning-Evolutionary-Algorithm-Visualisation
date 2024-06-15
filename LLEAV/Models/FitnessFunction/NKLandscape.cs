using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.FitnessFunction
{
    public class NKLandscape : AFitnessFunctionNoArg
    {
        public override string Depiction { get => "NK Landscape (K = " + k + ")"; }

        protected int k { get; set; } = 3;

        protected static double[] VALUES = [
            4, // 000
            3, // 001
            3, // 010
            -1,// 011
            3, // 100
            -1, // 101
            -1, // 110
            5, // 111
        ];

        public override double Fitness(Solution solution)
        {
            BitList bits = solution.Bits;
            double fitness = 0;

            for (int i = 0; i < bits.NumberBits; i++)
            {
                bool[] values = new bool[k];

                for(int j = 0; j < k; j++)
                {
                    values[j] = bits.Get((i + j) % bits.NumberBits);
                }

                fitness += f(values);
            }
            return fitness;

        }

        private double f(bool[] values)
        {
            int index = 0;
            for (int i = 0; i < values.Length; i++)
            {
                index += (values[i] ? 1 : 0) << i;
            }
            return VALUES[index];
        }

        public override string GetSolutionLengthValidationErrorMessage(int solutionLength)
        {
            return "Solution need to be at least " + k + " bits long.";
        }

        public override bool ValidateSolutionLength(int solutionLength)
        {
            return solutionLength >= k;
        }

    }
}
