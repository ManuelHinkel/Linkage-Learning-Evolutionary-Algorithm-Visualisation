﻿using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.FitnessFunction
{
    public class NKLandscape : IFitnessFunction
    {
        private const int K = 2;

        private static double[] VALUES = [
            0,
            2,
            1,
            0,
            1,
            2,
            1,
            0,
        ];

        public NKLandscape()
        {
            /*int count = 1 << (K + 1);
            VALUES = new double[count];

            Random r = new Random();
            for(int i = 0; i < count; i++)
            {
                VALUES[i] = r.NextDouble();
            }*/
        }

        public double Fitness(Solution solution)
        {
            BitList bits = solution.Bits;
            double fitness = 0;

            for (int i = 0; i < bits.NumberBits; i++)
            {
                bool[] values = new bool[K+1];

                for(int j = 0; j <K+1; j++)
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

        public string GetValidationErrorMessage(int solutionLength)
        {
            return "Solution need to be at least " + (K + 1) + " bits long.";
        }

        public bool ValidateSolutionLength(int solutionLength)
        {
            return solutionLength > K;
        }
    }
}
