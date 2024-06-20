using LLEAV.ViewModels.Controls;
using System;
using System.Collections.Generic;

namespace LLEAV.Models.Algorithms.MIP.StateChange
{

    /// <summary>
    /// Visualisation data specifically for the P3 or MIP algorithm.
    /// </summary>
    public class MIPVisualisationData() : IVisualisationData
    {
        public Solution? CurrentSolution { get; set; }
        public Solution? CurrentDonor { get; set; }
        public Solution? Merged { get; set; }

        public Population? ActivePopulation { get; set; }
        public Population? ViewedPopulation { get; set; }

        public Cluster? ActiveCluster { get; set; }
        public IList<Tuple<SolutionWrapper, SolutionWrapper>> Solutions { get; set; } = new List<Tuple<SolutionWrapper, SolutionWrapper>>();
        public IList<SolutionWrapper> Donors { get; set; } = new List<SolutionWrapper>();
        public bool IsMerging { get; set; }
        public bool IsApplyingCrossover { get; set; }

        public IVisualisationData Clone()
        {
            MIPVisualisationData clone = new MIPVisualisationData();

            clone.IsApplyingCrossover = IsApplyingCrossover;
            clone.IsMerging = IsMerging;

            clone.Donors = new List<SolutionWrapper>(Donors);
            clone.Solutions = new List<Tuple<SolutionWrapper, SolutionWrapper>>(Solutions);
            clone.ActiveCluster = ActiveCluster;
            clone.ViewedPopulation = ViewedPopulation;
            clone.ActivePopulation = ActivePopulation;
            clone.Merged = Merged;
            clone.CurrentDonor = CurrentDonor;
            clone.CurrentSolution = CurrentSolution;

            return clone;
        }
    }

    /// <summary>
    ///  State change interface specifically for the P3 or MIP algorithm.
    /// </summary>
    public interface IMIPStateChange : IStateChange
    {
        public Tuple<IList<string>, Message> Apply(IterationData state, MIPVisualisationData visualisationData, bool onlyOperateOnData = false);
    }
}
