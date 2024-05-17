using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Util
{
    public class ByteUtil
    {
        public static void WriteIntToBuffer(int value, byte[] buffer, int index)
        {
            buffer[index] = (byte)value;
            buffer[index + 1] = (byte)(value >> 8);
            buffer[index + 2] = (byte)(value >> 16);
            buffer[index + 3] = (byte)(value >> 24);
        }

        public static void WriteDoubleToBuffer(double value, byte[] buffer, int index)
        {
            BitConverter.GetBytes(value).CopyTo(buffer, index);
        }


        public static void WriteByteArrayToBuffer(byte[] values, byte[] buffer, int index)
        {
           for (int i = 0; i < values.Length; i++)
            {
                buffer[index + i] = values[i];
            }
        }
    }
}
