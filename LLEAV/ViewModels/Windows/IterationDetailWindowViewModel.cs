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
    /// <summary>
    /// ViewModel for managing detailed iteration data in a window, based on different algorithm types.
    /// </summary>
    public class IterationDetailWindowViewModel : ViewModelBase
    {

        private IterationDepictionViewModelBase? _contentViewModel;

        /// <summary>
        /// Gets or sets the content view model for displaying iteration details.
        /// </summary>
        public IterationDepictionViewModelBase ContentViewModel
        {
            get => _contentViewModel;
            private set => this.RaiseAndSetIfChanged(ref _contentViewModel, value);
        }

        /// <summary>
        /// Sets detailed data for the iteration view model based on the algorithm type.
        /// </summary>
        /// <param name="stateChanges">List of state changes during iterations.</param>
        /// <param name="workingCopy">Working copy of iteration data.</param>
        /// <param name="runData">Data related to the current run of the algorithm.</param>
        public void SetDetailedData(IList<IStateChange> stateChanges, IterationData workingCopy, RunData runData)
        {
            Stop();
            bool running = false;
            if (ContentViewModel != null)
            {
                running = ContentViewModel.Running;
            }
            switch (runData.Algorithm.AlgorithmType)
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

        /// <summary>
        /// Stops the current content view model if it is running.
        /// </summary>
        public void Stop()
        {
            if (_contentViewModel != null)
            {
                _contentViewModel!.Stop();
            }
        }

        /// <summary>
        /// Changes the solution depiction.
        /// </summary>
        public void ChangeSolutionDepiction()
        {
            if (_contentViewModel != null)
            {
                _contentViewModel.ChangeSolutionDepiction();
            }
        }

    }
}
