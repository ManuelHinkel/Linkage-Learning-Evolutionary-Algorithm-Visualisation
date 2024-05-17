using LLEAV.Models;
using System;

namespace LLEAV.Util
{
    public class BitList
    {
        public int NumberBits { get; private set; }
        private long[] bitArray;

        public BitList(int numberBits)
        {
            NumberBits = numberBits;
            bitArray = new long[1 + numberBits / 64];
        }

        public BitList(int numberBits, byte[] bytes)
        {
            NumberBits = numberBits;
            bitArray = new long[1 + numberBits / 64];
            FromByteArray(bytes);
        }

        public void Set(int index, bool value = true)
        {
            if (index >= NumberBits) { throw new ArgumentOutOfRangeException("index out of range"); }

            int slot = index / 64;
            int slotIndex = index % 64;
            long mask = 1L << slotIndex;
            if (value)
            {
                bitArray[slot] = bitArray[slot] | mask;
            }
            else
            {
                mask = ~mask;
                bitArray[slot] = bitArray[slot] & mask;
            }
        }

        public bool Get(int index)
        {
            if (index >= NumberBits) { throw new ArgumentOutOfRangeException("index out of range"); }
            int slot = index / 64;
            int slotIndex = index % 64;
            long mask = 1L << slotIndex;
            return (bitArray[slot] & mask) > 0;
        }

        public void Flip(int index)
        {
            if (index >= NumberBits) { throw new ArgumentOutOfRangeException("index out of range"); }
            int slot = index / 64;
            int slotIndex = index % 64;
            long mask = 1L << slotIndex;
            bitArray[slot] = bitArray[slot] ^ mask;
        }

        public BitList Clone()
        {
            BitList clone = new BitList(NumberBits);

            for (int i = 0; i < bitArray.Length; i++)
            {
                clone.bitArray[i] = bitArray[i];
            }

            return clone;
        }

        public static BitList Merge(BitList x, BitList donor, Cluster c)
        {
            // Clear all bits for which donation happens
            BitList t = x & !c.Mask;
            // Mask donor so only wanted bits get donated
            BitList donorBits = donor & c.Mask;
            return t | donorBits;
        }

        public static BitList operator &(BitList a, BitList b)
        {
            if (a == null || b == null)
            {
                throw new ArgumentException("argument is null");
            }
            if (a.NumberBits != b.NumberBits)
            {
                throw new ArgumentException("length of arguments differ");
            }
            BitList newBit = new BitList(a.NumberBits);

            for (int i = 0; i < newBit.bitArray.Length; i++)
            {
                newBit.bitArray[i] = a.bitArray[i] & b.bitArray[i];
            }

            return newBit;
        }

        public static BitList operator |(BitList a, BitList b)
        {
            if (a == null || b == null)
            {
                throw new ArgumentException("argument is null");
            }
            if (a.NumberBits != b.NumberBits)
            {
                throw new ArgumentException("length of arguments differ");
            }
            BitList newBit = new BitList(a.NumberBits);

            for (int i = 0; i < newBit.bitArray.Length; i++)
            {
                newBit.bitArray[i] = a.bitArray[i] | b.bitArray[i];
            }

            return newBit;
        }

        public static BitList operator !(BitList a)
        {
            if (a == null)
            {
                throw new ArgumentException("argument is null");
            }

            BitList newBit = new BitList(a.NumberBits);

            for (int i = 0; i < newBit.bitArray.Length; i++)
            {
                newBit.bitArray[i] = a.bitArray[i] ^ -1L;
            }

            return newBit;
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < NumberBits; i++)
            {
                if (i % 4 == 0) s += " ";
                s += Get(i) ? 1 : 0;
            }
            return s.Trim();
        }

        public byte[] ToByteArray()
        {
            int numberOfBytes = (int)Math.Ceiling(NumberBits / 8f);

            byte[] bytes = new byte[numberOfBytes];

            for (int i = 0; i <  numberOfBytes; i++)
            {
                bytes[i] = (byte)(bitArray[i / 8] >> ((i % 8) * 8));
            }

            return bytes;
        }

        private void FromByteArray(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                bitArray[i / 8] |= (long)bytes[i] << ((i % 8) * 8);
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is BitList) || ((BitList)obj).NumberBits != NumberBits)
            {
                return false;
            }
            else
            {
                bool same = true;
                for (int i = 0; i < bitArray.Length; i++)
                {
                    same = same & (bitArray[i] == ((BitList)obj).bitArray[i]);
                }
                return same;
            }
        }

        public override int GetHashCode()
        {
            int hc = bitArray.Length;
            foreach (long val in bitArray)
            {
                hc = unchecked(hc * 17 + (int)val);
                hc = unchecked(hc * 31 + (int)val >> 32);
            }
            return hc;
        }

        public static BitList RandomBitList(int numberBits, Random random)
        {
            BitList b = new BitList(numberBits);
            for (int i = 0; i < numberBits; i++)
            {
                b.Set(i, random.Next(0, 2) == 0 ? false : true);
            }
            return b;
        }
    }
}
