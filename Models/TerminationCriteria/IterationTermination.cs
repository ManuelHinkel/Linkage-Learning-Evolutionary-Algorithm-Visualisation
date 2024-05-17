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
