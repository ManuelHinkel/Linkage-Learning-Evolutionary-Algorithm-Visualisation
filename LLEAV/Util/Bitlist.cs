using LLEAV.Models;
using System;
using System.Diagnostics;

namespace LLEAV.Util
{

    /// <summary>
    /// Represents a bit list of any length.
    /// </summary>
    public class BitList
    {
        /// <summary>
        /// Gets the number of bits in the bit list.
        /// </summary>
        public int NumberBits { get; private set; }

        /// <summary>
        /// The underlying array storing the bits in unsigned longs.
        /// </summary>
        private ulong[] bitArray;


        /// <summary>
        /// Initializes a new instance with the specified number of bits.
        /// </summary>
        /// <param name="numberBits">The number of bits in the bit list.</param>
        public BitList(int numberBits)
        {
            NumberBits = numberBits;
            bitArray = new ulong[1 + numberBits / 64];
        }

        /// <summary>
        /// Initializes a new instance with the specified number of bits and a byte array.
        /// </summary>
        /// <param name="numberBits">The number of bits in the bit list.</param>
        /// <param name="bytes">The byte array to initialize the bit list with.</param>
        public BitList(int numberBits, byte[] bytes)
        {
            NumberBits = numberBits;
            bitArray = new ulong[1 + numberBits / 64];
            FromByteArray(bytes);
        }

        /// <summary>
        /// Sets the bit at the specified index to the specified value.
        /// </summary>
        /// <param name="index">The index of the bit to set.</param>
        /// <param name="value">The value to set the bit to.</param>
        public void Set(int index, bool value = true)
        {
            Debug.Assert(index < NumberBits); 

            int slot = index / 64;
            int slotIndex = index % 64;
            ulong mask = 1UL << slotIndex;
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

        /// <summary>
        /// Gets the value of the bit at the specified index.
        /// </summary>
        /// <param name="index">The index of the bit to get.</param>
        /// <returns>The value of the bit at the specified index.</returns>
        public bool Get(int index)
        {
            Debug.Assert(index < NumberBits);
            int slot = index / 64;
            int slotIndex = index % 64;
            ulong mask = 1UL << slotIndex;
            return (bitArray[slot] & mask) > 0;

        }

        /// <summary>
        /// Gets a range of bits from the specified start index (included) to the end index (excluded).
        /// </summary>
        /// <param name="startIndex">The start index of the range.</param>
        /// <param name="endIndex">The end index of the range.</param>
        /// <returns>A new <see cref="BitList"/> containing the bits in the specified range.</returns>
        public BitList Get(int startIndex, int endIndex)
        {
            int count = endIndex - startIndex;
            BitList range = new BitList(count);

            for (int i = 0; i < count; i++)
            {
                if (Get(i + startIndex))
                {
                    range.Set(i);
                }
            }

            return range; 
        }

        /// <summary>
        /// Flips the value of the bit at the specified index.
        /// </summary>
        /// <param name="index">The index of the bit to flip.</param
        public void Flip(int index)
        {
            Debug.Assert(index < NumberBits);
            int slot = index / 64;
            int slotIndex = index % 64;
            ulong mask = 1UL << slotIndex;
            bitArray[slot] = bitArray[slot] ^ mask;
        }


        /// <summary>
        /// Creates a copy of a bit list.
        /// </summary>
        /// <returns>The copy of the bit list.</returns>
        public BitList Clone()
        {
            BitList clone = new BitList(NumberBits);

            for (int i = 0; i < bitArray.Length; i++)
            {
                clone.bitArray[i] = bitArray[i];
            }

            return clone;
        }

        /// <summary>
        /// Merges two bit lists based on a cluster.
        /// </summary>
        /// <param name="x">The original bit list.</param>
        /// <param name="donor">The donor bit list.</param>
        /// <param name="c">The cluster used for merging.</param>
        /// <returns>A new bit lsit resulting from the merge.</returns>
        public static BitList Merge(BitList x, BitList donor, Cluster c)
        {
            // Clear all bits for which donation happens
            BitList t = x & !c.Mask;
            // Mask donor so only wanted bits get donated
            BitList donorBits = donor & c.Mask;
            return t | donorBits;
        }

        /// <summary>
        /// Performs a bitwise AND operation between two bit lists.
        /// </summary>
        /// <param name="a">The first bit list.</param>
        /// <param name="b">The second bit list.</param>
        /// <returns>A new bit list resulting from the bitwise AND operation.</returns>
        public static BitList operator &(BitList a, BitList b)
        {
            Debug.Assert(a != null && b != null);
            Debug.Assert(a.NumberBits == b.NumberBits);
            BitList newBit = new BitList(a.NumberBits);

            for (int i = 0; i < newBit.bitArray.Length; i++)
            {
                newBit.bitArray[i] = a.bitArray[i] & b.bitArray[i];
            }

            return newBit;
        }

        /// <summary>
        /// Performs a bitwise OR operation between two bit lists.
        /// </summary>
        /// <param name="a">The first bit list.</param>
        /// <param name="b">The second bit list.</param>
        /// <returns>A new bit list resulting from the bitwise OR operation.</returns>
        public static BitList operator |(BitList a, BitList b)
        {
            Debug.Assert(a != null && b != null);
            Debug.Assert(a.NumberBits == b.NumberBits);
            BitList newBit = new BitList(a.NumberBits);

            for (int i = 0; i < newBit.bitArray.Length; i++)
            {
                newBit.bitArray[i] = a.bitArray[i] | b.bitArray[i];
            }

            return newBit;
        }

        /// <summary>
        /// Performs a bitwise NOT operation on a bit list.
        /// </summary>
        /// <param name="a">The bit list to negate.</param>
        /// <returns>A new bit list resulting from the bitwise NOT operation.</returns>
        public static BitList operator !(BitList a)
        {
            Debug.Assert(a != null);

            BitList newBit = new BitList(a.NumberBits);

            for (int i = 0; i < newBit.bitArray.Length; i++)
            {
                newBit.bitArray[i] = a.bitArray[i] ^ ulong.MaxValue;
            }

            return newBit;
        }

        /// <summary>
        /// Returns a string representation of the bit list.
        /// </summary>
        /// <returns>A string that represents the bit list.</returns>
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

        /// <summary>
        /// Converts the bit list to a byte array representation for saving.
        /// </summary>
        /// <returns>A byte array representing the bit list.</returns>
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

        /// <summary>
        /// Initializes the bit list from a byte array representation.
        /// </summary>
        /// <param name="bytes">The byte array to initialize the bit list with.</param>
        private void FromByteArray(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                bitArray[i / 8] |= (ulong)bytes[i] << ((i % 8) * 8);
            }
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current bit list.
        /// </summary>
        /// <param name="obj">The object to compare with the current bit list.</param>
        /// <returns><c>true</c> if the specified object is equal to the current bit list; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Returns a hash code for the bit list.
        /// </summary>
        /// <returns>A hash code for the current bit list.</returns>
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

        /// <summary>
        /// Creates a new random bit list with the specified number of bits.
        /// </summary>
        /// <param name="numberBits">The number of bits in the bit list.</param>
        /// <param name="random">The random number generator to use for creating the bit list.</param>
        /// <returns>A new bit list with random bits.</returns>
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
