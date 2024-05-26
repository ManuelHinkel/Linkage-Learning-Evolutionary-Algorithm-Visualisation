using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.FitnessFunction
{
    public class DeceptiveTrap : IFitnessFunction
    {
        private const int K = 7;

        public double Fitness(Solution solution)
        {
            int blocks = (int)Math.Ceiling((double)solution.Bits.NumberBits / K);
            double fitness = 0;
            for (int i = 0; i < blocks; i++)
            {
                fitness += Trap(CountOnesInBlock(i, solution.Bits));
            }
            return fitness;
        }

        private double Trap(int t)
        {
            if (t == K)
            {
                return K;
            } else
            {
                return K - 1 - t;
            }
        }

        private int CountOnesInBlock(int block, BitList bits)
        {
            int numberOfOnes = 0;

            int start = block * K;
            int end = start + K;

            for(int i = start; i < Math.Min(end, bits.NumberBits); i++)
            {
                if (bits.Get(i))
                {
                    numberOfOnes++;
                }
            }
            return numberOfOnes;
        }
    }
}
