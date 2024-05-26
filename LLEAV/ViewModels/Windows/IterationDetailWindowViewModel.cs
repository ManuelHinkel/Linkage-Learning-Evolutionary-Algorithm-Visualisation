using LLEAV.Models;
using LLEAV.Models.Algorithms;
using LLEAV.Models.Algorithms.GOM.StateChange;
using LLEAV.Models.Algorithms.MIP.StateChange;
using LLEAV.Models.Algorithms.ROM.StateChange;
using LLEAV.ViewModels.Controls.IterationDepictions;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;

namespace LLEAV.ViewModels.Windows
{
    public class IterationDetailWindowViewModel : ViewModelBase
    {

        private IterationDepictionViewModelBase? _contentViewModel;

        public IterationDepictionViewModelBase ContentViewModel
        {
            get => _contentViewModel;
            private set => this.RaiseAndSetIfChanged(ref _contentViewModel, value);
        }

        public void SetDetailedData(IList<IStateChange> stateChanges, IterationData workingCopy, RunData runData)
        {
            Stop();
            bool running = false;
            if (ContentViewModel != null)
            {
                running = ContentViewModel.Running;
            }
            switch (runData.Algorithm.GetAlgorithmType())
            {
                case AlgorithmType.MIP:
                    ContentViewModel = new MIPIterationViewModel(stateChanges.Cast<IMIPStateChange>().ToList(), workingCopy);
                    break;
                case AlgorithmType.GOM:
                    ContentViewModel = new GOMIterationViewModel(stateChanges.Cast<IGOMStateChange>().ToList(), workingCopy);
                    break;
                case AlgorithmType.ROM:
                    ContentViewModel = new ROMIterationViewModel(stateChanges.Cast<IROMStateChange>().ToList(), workingCopy);
                    break;
            }
            ContentViewModel!.Running = running;
        }

        public void Stop()
        {
            if (_contentViewModel!= null)
            {
                _contentViewModel!.Stop();
            }
        }

        public void ChangeSolutionDepiction()
        {
            if (_contentViewModel != null)
            {
                _contentViewModel.ChangeSolutionDepiction();
            }
        }

    }
}
