using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.TerminationCriteria
{
    public class IterationTermination : AbstractTerminationCriteria
    {
        private int _iteration;

        public IterationTermination(string parameter) : base(parameter)
        {
            if (IsValid)
            {
                _iteration = (int)argument;
            }
        }

        public IterationTermination(byte[] bytes) : base(bytes)
        {
            argument = BitConverter.ToInt32(bytes, 0);
            _iteration = (int)argument;
        }

        public override byte[] ConvertArgumentToBytes()
        {
            byte[] bytes = new byte[4];
            ByteUtil.WriteIntToBuffer(_iteration, bytes, 0);
            return bytes;
        }

        public override Type GetArgumentType()
        {
            return typeof(int);
        }

        public override bool ShouldTerminate(IterationData iteration)
        {
            return iteration.Iteration >= _iteration;
        }

    }
}
