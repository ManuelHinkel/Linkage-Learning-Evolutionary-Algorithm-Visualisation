using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLEAV.Models.Algorithms;
using LLEAV.ViewModels;
using LLEAV.ViewModels.Controls;

namespace LLEAV.Models.Algorithms.MIP.StateChange
{
    public class MIPVisualisationData()
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
    }
    public interface IMIPStateChange : IStateChange
    {
        public Tuple<IList<string>,string> Apply(IterationData state, MIPVisualisationData visualisationData);
    }
}
