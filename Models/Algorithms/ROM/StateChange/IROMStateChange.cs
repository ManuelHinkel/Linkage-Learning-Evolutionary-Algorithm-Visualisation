using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LLEAV.ViewModels;
using LLEAV.ViewModels.Controls;

namespace LLEAV.Models.Algorithms.ROM.StateChange
{
    public class ROMVisualisationData(): IVisualisationData
    {
        public Solution? CurrentSolution1 { get; set; }
        public Solution? CurrentSolution2 { get; set; }
        public Solution? CurrentDonor1 { get; set; }
        public Solution? CurrentDonor2 { get; set; }
        public Cluster? ActiveCluster { get; set; }
        public bool IsMerging { get; set; }
        public bool IsFitnessIncreasing { get; set; }
        public bool IsFitnessDecreasing { get; set; }

        public IList<SolutionWrapper> Solutions { get; set; }
        public ObservableCollection<SolutionWrapper> NextIteration { get; set; } = new ObservableCollection<SolutionWrapper>();

        public IVisualisationData Clone()
        {
            ROMVisualisationData clone = new ROMVisualisationData();

            clone.NextIteration = new ObservableCollection<SolutionWrapper>(NextIteration);
            clone.Solutions = new List<SolutionWrapper>(Solutions);
            clone.IsFitnessDecreasing = IsFitnessDecreasing;
            clone.IsFitnessIncreasing = IsFitnessIncreasing;
            clone.IsMerging = IsMerging;
            clone.ActiveCluster = ActiveCluster;
            clone.CurrentSolution1 = CurrentSolution1;
            clone.CurrentSolution2 = CurrentSolution2;
            clone.CurrentDonor1 = CurrentDonor1;
            clone.CurrentDonor2 = CurrentDonor2;

            return clone;
        }
    }
    public interface IROMStateChange : IStateChange
    {
        public Tuple<IList<string>, string> Apply(IterationData state, ROMVisualisationData visualisationData, bool onlyOperateOnData = false);
    }
}
