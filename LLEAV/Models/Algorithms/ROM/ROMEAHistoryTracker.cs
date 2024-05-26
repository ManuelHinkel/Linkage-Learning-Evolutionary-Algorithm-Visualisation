using LLEAV.Models;
using LLEAV.Models.Algorithms;
using LLEAV.Models.Algorithms.ROM.StateChange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.ROM
{
    public class ROMEAHistoryTracker
    {
        public IList<IStateChange> StateChangeHistory { get; } = new List<IStateChange>();

        public void ChangeFOSCluster(Cluster cluster)
        {
            StateChangeHistory.Add(new ClusterChange(cluster));
        }

        public void UpdateFOS(Population population)
        {
            StateChangeHistory.Add(new FOSChange(population));
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

        public void Crossover(Solution o0, Solution o1)
        {
            StateChangeHistory.Add(new MergeChange(o0, o1));
        }

        public void FitnessIncrease(Solution p0, Solution p1)
        {
            StateChangeHistory.Add(new FitnessIncreaseChange(p0, p1));
        }

        public void FitnessDecrease(Solution o0, Solution o1)
        {
            StateChangeHistory.Add(new FitnessDecreaseChange(o0, o1));
        }

        public void ChangeActiveSolutions(Solution o0, Solution o1, Solution p0, Solution p1)
        {
            StateChangeHistory.Add(new ActiveSolutionsChange(o0, o1, p0, p1));
        }
    }
}
