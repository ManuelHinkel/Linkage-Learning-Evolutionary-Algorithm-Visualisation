using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLEAV.Models.Algorithms;
using LLEAV.ViewModels;
using LLEAV.ViewModels.Controls;

namespace LLEAV.Models.Algorithms.GOM.StateChange
{
    public class GOMVisualisationData()
    {
        public Solution? CurrentSolution { get; set; }
        public Solution? CurrentDonor { get; set; }
        public Solution? Merged { get; set; }

        public Cluster? ActiveCluster { get; set; }
        public bool IsMerging { get; set; }
        public bool IsApplyingCrossover { get; set; }

        public IList<SolutionWrapper> Solutions { get; set; }
        public ObservableCollection<SolutionWrapper> NextIteration { get; set; } = new ObservableCollection<SolutionWrapper>();
    }
    public interface IGOMStateChange : IStateChange
    {
        public Tuple<IList<string>, string> Apply(IterationData state, GOMVisualisationData visualisationData);
    }
}
