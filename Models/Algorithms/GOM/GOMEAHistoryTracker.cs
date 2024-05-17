using LLEAV.Models;
using LLEAV.Models.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.GOM
{
    public class GOMEAHistoryTracker
    {
        public IList<IStateChange> StateChangeHistory { get; private set; }

        public void ChangeFOSCluster(Cluster cluster)
        {

        }

        public void ChangeDonor(Solution donor)
        {

        }

        public void ChangeActiveSolution(Solution active)
        {
            
        }

        public void ApplyCrossover()
        {

        }

        public void RevertCrossover()
        {

        }
    }
}
