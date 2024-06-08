using LLEAV.Models.FitnessFunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.LocalSearchFunction
{
    public abstract class ALocalSearchFunction
    {
        public abstract string Depiction { get; }
        public abstract Solution Execute(Solution solution, AFitnessFunction fitnessFunction, Random random);
    }
}
