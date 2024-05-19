using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LLEAV.ViewModels;
using LLEAV.ViewModels.Controls;

namespace LLEAV.Models.Algorithms.ROM.StateChange
{
    public class ROMVisualisationData()
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
    }
    public interface IROMStateChange : IStateChange
    {
        public Tuple<IList<string>, string> Apply(IterationData state, ROMVisualisationData visualisationData);
    }
}
