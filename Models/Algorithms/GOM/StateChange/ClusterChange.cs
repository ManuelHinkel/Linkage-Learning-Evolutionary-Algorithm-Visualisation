using LLEAV.Models;
using LLEAV.Models.Algorithms.MIP.StateChange;
using LLEAV.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.GOM.StateChange
{
    public class ClusterChange : IGOMStateChange
    {
        private Cluster _cluster;
        public ClusterChange(Cluster cluster)
        {
            _cluster = cluster;
        }

        public Tuple<IList<string>, string> Apply(IterationData state, GOMVisualisationData visualisationData, bool onlyOperateOnData = false)
        {
            if (!onlyOperateOnData)
            {
                GlobalManager.Instance.SelectCluster(0, _cluster);
            }
            visualisationData.ActiveCluster = _cluster;

            return new Tuple<IList<string>, string>(["CurrentDonor", "CurrentSolution"], "Changed the cluster to: \n" + _cluster.Mask);
        }
    }
}
