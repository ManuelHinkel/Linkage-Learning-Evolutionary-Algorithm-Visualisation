using LLEAV.Models;
using LLEAV.Models.Algorithms.ROM.StateChange;
using System.Collections.Generic;

namespace LLEAV.ViewModels.Controls.IterationDepictions
{
    public class ROMIterationViewModel : IterationDepictionViewModelBase
    {
        public IList<IROMStateChange> StateChanges { get; set; }


        public IList<SolutionWrapper> Solutions
        {
            get { return null; }
        }



        public IList<SolutionWrapper> SolutionsOfNextPopulation
        {
            get { return null; }
        }

        public SolutionWrapper CurrentSolution1
        {
            get { return null; }
        }

        public SolutionWrapper CurrentSolution2
        {
            get { return null; }
        }

        public SolutionWrapper CurrentDonor1
        {
            get { return null; }
        }

        public SolutionWrapper CurrentDonor2
        {
            get { return null; }
        }

        public ROMIterationViewModel(List<IROMStateChange> stateChanges, IterationData workingData)
        {
            StateChanges = stateChanges;
            WorkingData = workingData;
        }

        public ROMIterationViewModel()
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
