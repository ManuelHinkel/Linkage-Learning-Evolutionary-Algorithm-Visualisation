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
    public class IterationTermination : ATerminationCriteria
    {
        public override string Depiction { get; } = "Iteration Termination";
        public override Type ArgumentType { get; } = typeof(int);
        private int _iteration;

        public override byte[] ConvertArgumentToBytes()
        {
            byte[] bytes = new byte[4];
            ByteUtil.WriteIntToBuffer(_iteration, bytes, 0);
            return bytes;
        }

        public override bool CreateArgumentFromBytes(byte[] bytes)
        {
            _iteration = BitConverter.ToInt32(bytes, 0);
            return true;
        }

        public override bool CreateArgumentFromString(string arg)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
            if (converter.IsValid(arg))
            {
                _iteration = (int)converter.ConvertTo(arg, typeof(int));
                return true;
            }
            return false;
        }


        public override string GetTerminationString()
        {
            return "Iteration " + _iteration + " reached.";
        }

        public override bool ShouldTerminate(IterationData iteration)
        {
            return iteration.Iteration >= _iteration;
        }

    }
}
