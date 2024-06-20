using LLEAV.Util;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LLEAV.Models
{
    /// <summary>
    /// Represents a cluster of bits.
    /// </summary>
    public class Cluster : IEnumerable<Cluster>
    {
        private SortedSet<int> bits = new SortedSet<int>();

        /// <summary>
        /// Gets the size of the cluster.
        /// </summary>
        public int NumberOfBits { get; private set; }

        /// <summary>
        /// Gets the bit mask representing the cluster.
        /// </summary>
        public BitList Mask { get; private set; }

        /// <summary>
        /// Initializes a new instance of a cluster with the specified bits and total number of bits.
        /// </summary>
        /// <param name="bits">The list of bits in the cluster.</param>
        /// <param name="numberOfBits">The total number of bits.</param>
        public Cluster(List<int> bits, int numberOfBits)
        {
            NumberOfBits = numberOfBits;
            Mask = new BitList(numberOfBits);
            foreach (int bit in bits)
            {
                this.bits.Add(bit);
                Mask.Set(bit);
            }
        }

        /// <summary>
        /// Gets the count of bits in the cluster.
        /// </summary>
        /// <returns>The count of bits in the cluster.</returns>
        public int Count()
        {
            return bits.Count;
        }

        /// <summary>
        /// Returns the union of the current cluster and another cluster.
        /// </summary>
        /// <param name="other">The other cluster to union with.</param>
        /// <returns>A new cluster representing the union of the two clusters.</returns>
        public Cluster Union(Cluster other)
        {
            Debug.Assert(NumberOfBits == other.NumberOfBits);

            List<int> bits = this.bits.ToList();
            bits.AddRange(other.bits);

            return new Cluster(bits, NumberOfBits);
        }

        /// <summary>
        /// Determines whether the current cluster contains another cluster.
        /// </summary>
        /// <param name="other">The other cluster to check for containment.</param>
        /// <returns><c>true</c> if the current cluster contains the other cluster; otherwise, <c>false</c>.</returns>
        public bool Contains(Cluster other)
        {
            Debug.Assert(NumberOfBits == other.NumberOfBits);

            return (Mask & other.Mask).Equals(other.Mask);
        }

        /// <summary>
        /// Generates all possible bit strings covered by the cluster.
        /// </summary>
        /// <returns>An enumerable of bit lists representing all possible bit strings.</returns>
        public IEnumerable<BitList> PossibleStrings()
        {
            List<BitList> temporaryStrings = [new BitList(NumberOfBits)];
            foreach (int bit in bits)
            {
                List<BitList> newTemp = new List<BitList>();
                foreach (BitList temporarySolution in temporaryStrings)
                {
                    //Add once with 0 at position bit
                    newTemp.Add(temporarySolution.Clone());
                    temporarySolution.Set(bit);
                    //Add once with 1 at position bit
                    newTemp.Add(temporarySolution);
                }
                temporaryStrings = newTemp;

            }

            return temporaryStrings;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current cluster.
        /// </summary>
        /// <param name="obj">The object to compare with the current cluster.</param>
        /// <returns><c>true</c> if the specified object is equal to the current cluster; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is Cluster))
            {
                return false;
            }
            else
            {
                return Mask.Equals(((Cluster)obj).Mask);
            }
        }

        /// <summary>
        /// Returns a string that represents the current cluster.
        /// </summary>
        /// <returns>A string that represents the current cluster.</returns>
        public override string ToString()
        {
            return "Cluster: " + Mask.ToString();
        }

        /// <summary>
        /// Gets a string representation of the bit positions in the cluster.
        /// </summary>
        /// <returns>A string representation of the bit positions in the cluster.</returns>
        public string BitPositions()
        {
            return string.Join(", ", bits.Select(x => x.ToString()));
        }

        /// <summary>
        /// Returns an enumerator that iterates through a cluster for each bit.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the bits of the cluster.</returns>
        public IEnumerator<Cluster> GetEnumerator()
        {
            foreach (int bit in bits)
            {
                yield return new Cluster([bit], NumberOfBits);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        /// <summary>
        /// Inverts the bits in the cluster.
        /// </summary>
        /// <param name="a">The cluster to invert.</param>
        /// <returns>A new cluster representing the inverted bits.</returns>
        public static Cluster operator !(Cluster a)
        {
            Debug.Assert(a != null);

            List<int> bits = Enumerable.Range(0, a.NumberOfBits).ToList();

            foreach (int bit in a.bits)
            {
                bits.Remove(bit);
            }

            Cluster inverted = new Cluster(bits, a.NumberOfBits);

            return inverted;
        }

        /// <summary>
        /// Gets the position of the first bit in the cluster.
        /// </summary>
        /// <returns>The position of the first bit in the cluster.</returns>
        public int PositionOfFirstBit()
        {
            if (bits.Count == 0) return 0;
            return bits.ElementAt(0);
        }
    }
}
