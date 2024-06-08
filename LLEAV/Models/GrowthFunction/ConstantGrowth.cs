using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.GrowthFunction
{
    public class ConstantGrowth : AGrowthFunction
    {
        public override string Depiction { get; } = "Constant Growth";
        public override int GetNumberOfNewSolutions(int iteration)
        {
            return 1;
        }
    }
}
