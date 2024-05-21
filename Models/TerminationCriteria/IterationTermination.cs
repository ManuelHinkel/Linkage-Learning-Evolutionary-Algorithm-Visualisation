using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.TerminationCriteria
{
    public class IterationTermination : ITerminationCriteria
    {
        private int _iteration;

        public byte[] ConvertArgumentToBytes()
        {
            byte[] bytes = new byte[4];
            ByteUtil.WriteIntToBuffer(_iteration, bytes, 0);
            return bytes;
        }

        public bool CreateArgumentFromBytes(byte[] bytes)
        {
            _iteration = BitConverter.ToInt32(bytes, 0);
            return true;
        }

        public bool CreateArgumentFromString(string arg)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
            if (converter.IsValid(arg))
            {
                _iteration = (int)converter.ConvertTo(arg, typeof(int));
                return true;
            }
            return false;
        }

        public Type GetArgumentType()
        {
            return typeof(int);
        }

        public bool ShouldTerminate(IterationData iteration)
        {
            return iteration.Iteration >= _iteration;
        }

    }
}
