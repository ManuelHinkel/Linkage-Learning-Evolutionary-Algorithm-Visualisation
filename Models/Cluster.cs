using LLEAV.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models
{
    public class Cluster : IEnumerable<Cluster>
    {
        private SortedSet<int> bits = new SortedSet<int>();
        public int NumberOfBits { get; private set; }

        public BitList Mask { get; private set; }

        public Cluster(List<int> bits, int numberOfBits)
        {
            this.NumberOfBits = numberOfBits;
            Mask = new BitList(numberOfBits);
            foreach (int bit in bits)
            {
                this.bits.Add(bit);
                Mask.Set(bit);
            }
        }

        public int Count()
        {
            return bits.Count;
        }

        public Cluster Union(Cluster other)
        {
            if (NumberOfBits != other.NumberOfBits)
            {
                throw new ArgumentException("number of bits of arguments differ");
            }

            List<int> bits = this.bits.ToList();
            bits.AddRange(other.bits);

            return new Cluster(bits, NumberOfBits);
        }

        public bool Contains(Cluster other)
        {
            if (NumberOfBits != other.NumberOfBits)
            {
                throw new ArgumentException("number of bits of arguments differ");
            }

            return (Mask & other.Mask).Equals(other.Mask);
        }

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

        public override string ToString()
        {
            return "Cluster: " + Mask.ToString();
        }

        public string BitPositions()
        {
            return string.Join(", ", bits.Select(x => x.ToString()));
        }

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

        public static Cluster operator !(Cluster a)
        {
            if (a == null)
            {
                throw new ArgumentException("argument is null");
            }

            List<int> bits = Enumerable.Range(0, a.NumberOfBits).ToList();

            foreach (int bit in a.bits)
            {
                bits.Remove(bit);
            }

            Cluster inverted = new Cluster(bits, a.NumberOfBits);

            return inverted;
        }
    }
}
