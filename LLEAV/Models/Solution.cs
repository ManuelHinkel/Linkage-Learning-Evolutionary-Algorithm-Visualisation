using LLEAV.Util;
using System;

namespace LLEAV.Models
{
    /// <summary>
    /// Represents a solution with a fitness value and a bit list.
    /// </summary>
    public class Solution
    {
        /// <summary>
        /// Gets or sets the fitness value of the solution.
        /// </summary>
        public double Fitness { get; set; }

        /// <summary>
        /// Gets or sets the bit list representing the solution.
        /// </summary>
        public BitList Bits { get; set; }

        /// <summary>
        /// Returns a string representation of the solution.
        /// </summary>
        /// <returns>A string that represents the solution.</returns>
        public override string ToString()
        {
            return Bits.ToString() + " Fitness: " + Fitness;
        }

        /// <summary>
        /// Returns a hash code for the solution.
        /// </summary>
        /// <returns>A hash code for the current solution.</returns>
        public override int GetHashCode()
        {
            return Bits.GetHashCode();
        }


        /// <summary>
        /// Creates a copy of a solution.
        /// </summary>
        /// <returns>The copy of the solution.</returns>
        public Solution Clone()
        {
            Solution clone = new Solution();
            clone.Fitness = Fitness;
            clone.Bits = Bits.Clone();
            return clone;
        }

        /// <summary>
        /// Merges two solutions based on a cluster.
        /// </summary>
        /// <param name="x">The original solution.</param>
        /// <param name="donor">The donor solution.</param>
        /// <param name="c">The cluster used for merging.</param>
        /// <returns>A new solution resulting from the merge.</returns>
        public static Solution Merge(Solution x, Solution donor, Cluster c)
        {
            Solution merged = new Solution();
            merged.Fitness = x.Fitness;
            merged.Bits = BitList.Merge(x.Bits, donor.Bits, c);
            return merged;
        }

        /// <summary>
        /// Creates a new random solution with the specified number of bits.
        /// </summary>
        /// <param name="numberBits">The number of bits in the solution.</param>
        /// <param name="random">The random number generator to use for creating the solution.</param>
        /// <returns>A new solution with random bits.</returns>
        public static Solution RandomSolution(int numberBits, Random random)
        {
            Solution newSolution = new Solution();
            newSolution.Bits = BitList.RandomBitList(numberBits, random);
            return newSolution;
        }
    }
}
