using LLEAV.Models.FitnessFunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.LocalSearchFunction
{
    public interface ILocalSearchFunction
    {
        public Solution Execute(Solution solution, IFitnessFunction fitnessFunction, Random random);
    }
}
