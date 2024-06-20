using System;

namespace LLEAV.Util
{
    /// <summary>
    /// Utility class for writing different data types to a byte array buffer.
    /// </summary>
    public class ByteUtil
    {
        /// <summary>
        /// Writes an integer value to a byte array buffer starting at the specified index.
        /// </summary>
        /// <param name="value">The integer value to write.</param>
        /// <param name="buffer">The byte array buffer.</param>
        /// <param name="index">The starting index in the buffer.</param>
        public static void WriteIntToBuffer(int value, byte[] buffer, int index)
        {
            buffer[index] = (byte)value;
            buffer[index + 1] = (byte)(value >> 8);
            buffer[index + 2] = (byte)(value >> 16);
            buffer[index + 3] = (byte)(value >> 24);
        }

        /// <summary>
        /// Writes a double value to a byte array buffer starting at the specified index.
        /// </summary>
        /// <param name="value">The double value to write.</param>
        /// <param name="buffer">The byte array buffer.</param>
        /// <param name="index">The starting index in the buffer.</param>
        public static void WriteDoubleToBuffer(double value, byte[] buffer, int index)
        {
            BitConverter.GetBytes(value).CopyTo(buffer, index);
        }


        /// <summary>
        /// Writes a byte array to another byte array buffer starting at the specified index.
        /// </summary>
        /// <param name="values">The byte array to write.</param>
        /// <param name="buffer">The byte array buffer.</param>
        /// <param name="index">The starting index in the buffer.</param>
        public static void WriteByteArrayToBuffer(byte[] values, byte[] buffer, int index)
        {
            for (int i = 0; i < values.Length; i++)
            {
                buffer[index + i] = values[i];
            }
        }
    }
}
