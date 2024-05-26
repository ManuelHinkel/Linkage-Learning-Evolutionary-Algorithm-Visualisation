using LLEAV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.FOSFunction
{
    public class UnivariateFOS : IFOSFunction
    {
        public FOS CalculateFOS(Population population, int numberOfBits)
        {
            IList<Cluster> clusters = new List<Cluster>();

            for (int i = 0; i < numberOfBits; i++)
            {
                clusters.Add(new Cluster([i], numberOfBits));
            }

            return new FOS(clusters, FOSType.UNIVARIATE);
        }
    }
}
