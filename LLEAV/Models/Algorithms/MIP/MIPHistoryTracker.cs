using LLEAV.Models;
using LLEAV.Models.Algorithms;
using LLEAV.Models.Algorithms.MIP.StateChange;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.MIP
{
    public class MIPHistoryTracker
    {
        public IList<IStateChange> StateChangeHistory { get; } = new List<IStateChange>();


        public void ChangeDonor(Solution donor)
        {
            StateChangeHistory.Add(new DonorChange(donor));
        }

        public void ChangeDonors(Population population)
        {
            StateChangeHistory.Add(new DonorListChange(population));
        }

        public void ChangeFOSCluster(Cluster cluster)
        {
            StateChangeHistory.Add(new ClusterChange(cluster));
        }

        public void ChangeActiveSolution(Solution active)
        {
            StateChangeHistory.Add(new ActiveSolutionChange(active));
        }

        public void ApplyCrossover(Solution merged)
        {
            StateChangeHistory.Add(new ApplyCrossoverChange(merged));
        }

        public void SetGeneratedSolutions(IList<Tuple<Solution, Solution>> generated)
        {
            StateChangeHistory.Add(new ParentsCreatedChange(generated));
        }

        public void Merge(Solution merged)
        {
            StateChangeHistory.Add(new MergeChange(merged));
        }

        public void AddedSolutionToNextPopulation(Population population)
        {
            StateChangeHistory.Add(new SolutionAddedToNextPopulationChange(population));
            SetViewedPopulation(population);
        }
        public void SetViewedPopulation(Population population)
        {
            StateChangeHistory.Add(new ViewedPopulationChanged(population));
        }


        public void UpdateFOS(Population population)
        {
            StateChangeHistory.Add(new FOSChange(population));
            SetViewedPopulation(population);
        }

        public void SetTermination(bool terminate)
        {
            StateChangeHistory.Add(new TerminationChange(terminate));
        }
    }
}
