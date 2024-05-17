using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.GrowthFunction
{
    public interface IGrowthFunction
    {
        public int GetNumberOfNewSolutions(int iteration);
    }
}
