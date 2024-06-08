using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.TerminationCriteria
{
    public abstract class ATerminationCriteria
    {
        public abstract string Depiction { get; }
        public abstract Type ArgumentType { get; }
        public abstract bool ShouldTerminate(IterationData iteration);
        public abstract bool CreateArgumentFromString(string arg);
        public abstract byte[] ConvertArgumentToBytes();
        public abstract bool CreateArgumentFromBytes(byte[] bytes);

        public abstract string GetTerminationString();
    }
}
