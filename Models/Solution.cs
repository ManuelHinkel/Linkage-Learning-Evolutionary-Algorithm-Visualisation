using LLEAV.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models
{
    public class Solution
    {
        public double Fitness { get; set; }
        public BitList Bits { get; set; }

        public override string ToString()
        {
            return Bits.ToString() + " Fitness: " + Fitness;
        }

        public override int GetHashCode()
        {
            return Bits.GetHashCode();
        }

        public Solution Clone()
        {
            Solution clone = new Solution();
            clone.Fitness = Fitness;
            clone.Bits = Bits.Clone();
            return clone;
        }

        public static Solution Merge(Solution x, Solution donor, Cluster c)
        {
            Solution merged = new Solution();
            merged.Bits = BitList.Merge(x.Bits, donor.Bits, c);
            return merged;
        }

        public static Solution RandomSolution(int numberBits, Random random)
        {
            Solution newSolution = new Solution();
            newSolution.Bits = BitList.RandomBitList(numberBits, random);
            return newSolution;
        }
    }
}
