using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.TerminationCriteria
{
    public class FitnessTermination : ATerminationCriteria
    {
        public override string Depiction { get; } = "Fitness Termination";

        public override Type ArgumentType { get; } = typeof(double);

        private double _fitness;
        private Solution? _reached;

        public override byte[] ConvertArgumentToBytes()
        {
            byte[] bytes = new byte[4];
            ByteUtil.WriteDoubleToBuffer(_fitness, bytes, 0);
            return bytes;
        }

        public override bool CreateArgumentFromBytes(byte[] bytes)
        {
            _fitness = BitConverter.ToDouble(bytes, 0);
            return true;
        }

        public override bool CreateArgumentFromString(string arg)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(double));
            if (converter.IsValid(arg))
            {
                _fitness = double.Parse(arg, CultureInfo.InvariantCulture);
                return true;
            }
            return false;
        }

        public override string GetTerminationString()
        {
            return "Fitness " + _fitness + " reached with Solution: " + _reached;
        }

        public override bool ShouldTerminate(IterationData iteration)
        {
            foreach(Population p in iteration.Populations)
            {
                foreach(Solution s in p)
                {
                    Debug.WriteLine(_fitness);
                    if (s.Fitness >= _fitness - 0.001)
                    {
                        _reached = s;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
