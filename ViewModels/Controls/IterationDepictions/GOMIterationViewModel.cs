using LLEAV.Models;
using LLEAV.Models.Algorithms.GOM.StateChange;
using System.Collections.Generic;

namespace LLEAV.ViewModels.Controls.IterationDepictions
{
    public class GOMIterationViewModel : IterationDepictionViewModelBase
    {
        public IList<IGOMStateChange> StateChanges { get; set; }


        public IList<SolutionWrapper> Solutions
        {
            get { return null; }
        }

        public IList<SolutionWrapper> SolutionsOfNextPopulation
        {
            get { return null; }
        }

        public SolutionWrapper CurrentSolution
        {
            get { return null; }
        }

        public SolutionWrapper CurrentDonor
        {
            get { return null; }
        }
        public SolutionWrapper Merged
        {
            get { return null; }
        }

        public GOMIterationViewModel(List<IGOMStateChange> stateChanges, IterationData workingData)
        {
            StateChanges = stateChanges;
            WorkingData = workingData;
        }

        public GOMIterationViewModel()
        {

        }

        public override void StepForward()
        {
            throw new System.NotImplementedException();
        }

        public override void StepBackward()
        {
            throw new System.NotImplementedException();
        }

        protected override void RaiseChanged(IList<string> properties)
        {
            throw new System.NotImplementedException();
        }

        protected override void GoToStateChange(int index)
        {
            throw new System.NotImplementedException();
        }
    }
}
