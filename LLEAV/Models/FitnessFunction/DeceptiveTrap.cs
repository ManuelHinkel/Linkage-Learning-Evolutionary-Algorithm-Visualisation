using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.FitnessFunction
{
    public class DeceptiveTrap : AFitnessFunction
    {
        public override string Depiction { get => "Deceptive Trap (K = " + _k + ")"; }
        public override bool EnableArg => true;

        private int _k = 7;

        public override double Fitness(Solution solution)
        {
            int blocks = (int)Math.Ceiling((double)solution.Bits.NumberBits / _k);
            double fitness = 0;
            for (int i = 0; i < blocks; i++)
            {
                fitness += Trap(CountOnesInBlock(i, solution.Bits));
            }
            return fitness;
        }

        private double Trap(int t)
        {
            if (t == _k)
            {
                return _k;
            } else
            {
                return _k - 1 - t;
            }
        }

        private int CountOnesInBlock(int block, BitList bits)
        {
            int numberOfOnes = 0;

            int start = block * _k;
            int end = start + _k;

            for(int i = start; i < Math.Min(end, bits.NumberBits); i++)
            {
                if (bits.Get(i))
                {
                    numberOfOnes++;
                }
            }
            return numberOfOnes;
        }

        public override bool ValidateSolutionLength(int solutionLength)
        {
            return solutionLength % _k == 0;
        }

        public override string GetSolutionLengthValidationErrorMessage(int solutionLength)
        {
            return "Solution length must be a multiple of: " + _k + "\nMaybe you ment: " + (solutionLength + (_k - (solutionLength % _k)));
        }

        public override bool CreateArgumentFromString(string arg)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
            if (converter.IsValid(arg))
            {
                _k = (int)converter.ConvertTo(arg, typeof(int));
                return _k > 0;
            }
            return false;
        }

        public override string GetArgValidationErrorMessage(string arg)
        {
            return "Blocksize must be a number greater 0.";
        }

        public override byte[] ConvertArgumentToBytes()
        {
            byte[] bytes = new byte[4];
            ByteUtil.WriteIntToBuffer(_k, bytes, 0);
            return bytes;
        }

        public override bool CreateArgumentFromBytes(byte[] bytes)
        {
            _k = BitConverter.ToInt32(bytes, 0);
            return true;
        }
    }
}
