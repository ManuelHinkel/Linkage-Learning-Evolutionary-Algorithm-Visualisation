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
    public class GOMVisualisationData(): IVisualisationData
    {
        public Solution? CurrentSolution { get; set; }
        public Solution? CurrentDonor { get; set; }
        public Solution? Merged { get; set; }
        public Cluster? ActiveCluster { get; set; }
        public bool IsMerging { get; set; }
        public bool IsApplyingCrossover { get; set; }

        public IList<SolutionWrapper> Solutions { get; set; }
        public ObservableCollection<SolutionWrapper> NextIteration { get; set; } = new ObservableCollection<SolutionWrapper>();
    
        public IVisualisationData Clone()
        {
            GOMVisualisationData clone = new GOMVisualisationData();

            clone.Solutions = new List<SolutionWrapper>(Solutions);
            clone.NextIteration = new ObservableCollection<SolutionWrapper>(NextIteration);

            clone.IsApplyingCrossover = IsApplyingCrossover;
            clone.IsMerging = IsMerging;
            clone.ActiveCluster = ActiveCluster;
            clone.Merged = Merged;
            clone.CurrentDonor = CurrentDonor; 
            clone.CurrentSolution = CurrentSolution;

            return clone;
        }
    }
    public interface IGOMStateChange : IStateChange
    {
        public Tuple<IList<string>, Message> Apply(IterationData state, GOMVisualisationData visualisationData, bool onlyOperateOnData = false);
    }
}
