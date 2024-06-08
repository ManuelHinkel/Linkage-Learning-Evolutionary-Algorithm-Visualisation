﻿using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.FitnessFunction
{
    public class HIFF : AFitnessFunction
    {

        private double _sum;

        public override string Depiction { get; } = "HIFF";

        public override double Fitness(Solution solution)
        {
            _sum = 0;
            F(solution.Bits);
            return _sum;
        }

        public override string GetValidationErrorMessage(int solutionLength)
        {
            return "Solution length mus be a power of two.";
        }

        public override bool ValidateSolutionLength(int solutionLength)
        {
            return (solutionLength != 0) && ((solutionLength & (solutionLength - 1)) == 0);
        }

        private int F(BitList bits)
        {
            /*if (bits.NumberBits == 2)
            {
                if (bits.Get(0) == bits.Get(1))
                {
                    _sum += 4;
                    return 2;
                }
                _sum += 2;
                return 0;
            } else
            {
                double left = F(bits.Get(0, bits.NumberBits / 2));
                double right = F(bits.Get(bits.NumberBits / 2, bits.NumberBits));
                
                if (left > 0 && right > 0)
                {
                    _sum += left + right;
                    return left + right;
                }

                return 0;
            }*/
            if (bits.NumberBits == 1)
            {
                _sum += 1;
                if (bits.Get(0))
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            } else
            {
                int left = F(bits.Get(0, bits.NumberBits / 2));
                int right = F(bits.Get(bits.NumberBits / 2, bits.NumberBits));

                if (left == right)
                {
                    _sum += 2 * Math.Abs(left);
                    return 2 * left;
                } else
                {
                    return 0;
                }
            }
        }
    }
}
