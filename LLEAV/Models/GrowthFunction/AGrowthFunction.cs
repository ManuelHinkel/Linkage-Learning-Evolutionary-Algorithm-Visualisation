using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.GrowthFunction
{
    public abstract class AGrowthFunction
    {
        public abstract string Depiction { get; }
        public abstract int GetNumberOfNewSolutions(int iteration);
    }
}
