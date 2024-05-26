using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.FitnessFunction
{
    public class HIFF : IFitnessFunction
    {

        private double _sum;

        public double Fitness(Solution solution)
        {
            _sum = 0;
            F(solution.Bits);
            return _sum;
        }

        public string GetValidationErrorMessage(int solutionLength)
        {
            return "Solution length mus be a power of two.";
        }

        public bool ValidateSolutionLength(int solutionLength)
        {
            return (solutionLength != 0) && ((solutionLength & (solutionLength - 1)) == 0);
        }

        private double F(BitList bits)
        {
            if (bits.NumberBits == 2)
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
            }
        }
    }
}
