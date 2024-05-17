using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.GrowthFunction
{
    public class LinearGrowth : IGrowthFunction
    {
        public int GetNumberOfNewSolutions(int iteration)
        {
            return iteration > 0 ? iteration : 1;
        }
    }
}
