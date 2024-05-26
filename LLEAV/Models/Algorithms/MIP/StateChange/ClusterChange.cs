using LLEAV.Models;
using LLEAV.ViewModels;
using LLEAV.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.MIP.StateChange
{
    public class ClusterChange : IMIPStateChange
    {
        private Cluster _cluster;
        public ClusterChange(Cluster cluster) 
        {
            _cluster = cluster;
        }

        public Tuple<IList<string>, Message> Apply(IterationData state, MIPVisualisationData visualisationData, bool onlyOperateOnData = false)
        {
            if (!onlyOperateOnData)
            {
                GlobalManager.Instance.SelectCluster(visualisationData.ViewedPopulation.PyramidIndex, _cluster);
            }
            visualisationData.ActiveCluster = _cluster;

            return new Tuple<IList<string>, Message>(["CurrentDonor", "CurrentSolution"],
                new Message("Changed the cluster to: \n" + _cluster.Mask, MessagePriority.INTERESTING));
        }
    }
}
