using LLEAV.Models;
using LLEAV.Models.Algorithms;
using LLEAV.Models.Algorithms.GOM.StateChange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.GOM
{
    public class GOMEAHistoryTracker
    {
        public IList<IStateChange> StateChangeHistory { get; } = new List<IStateChange>();

        public void ChangeActiveSolution(Solution active)
        {
            StateChangeHistory.Add(new ActiveSolutionChange(active));
        }

        public void ApplyCrossover(Solution merged)
        {
            StateChangeHistory.Add(new ApplyCrossoverChange(merged));
        }

        public void ChangeFOSCluster(Cluster cluster)
        {
            StateChangeHistory.Add(new ClusterChange(cluster));
        }

        public void ChangeDonor(Solution donor)
        {
            StateChangeHistory.Add(new DonorChange(donor));
        }

        public void UpdateFOS(Population population)
        {
            StateChangeHistory.Add(new FOSChange(population));
        }

        public void Merge(Solution merged)
        {
            StateChangeHistory.Add(new MergeChange(merged));
        }

        public void SetViewedPopulation(Population population)
        {
            StateChangeHistory.Add(new PresentedPopulationChanged(population));
        }

        public void AddSolutionToNextIteration(Solution solution)
        {
            StateChangeHistory.Add(new SolutionAddedToNextIterationChange(solution));
        }
   

        public void ApplyTournamentSelection(IList<Solution> solutions)
        {
            StateChangeHistory.Add(new TournamentSelectionChange(solutions));
        }

        public void SetTermination(bool terminate)
        {
            StateChangeHistory.Add(new TerminationChange(terminate));
        }

        public void IncreaseFitnessEvaluations(Solution[] solutions)
        {
            StateChangeHistory.Add(new EvaluationChange(solutions));
        }
    }
}
