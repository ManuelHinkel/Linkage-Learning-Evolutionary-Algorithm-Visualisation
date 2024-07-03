using Avalonia.Threading;
using LLEAV.Models;
using LLEAV.Models.Algorithms;
using LLEAV.Models.Persistence;
using LLEAV.ViewModels.Controls.PopulationDepictions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace LLEAV.ViewModels.Windows
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static readonly int[] TICK_STEPS = [1000, 500, 250, 100, 50];

        private int _iteration;
        /// <summary>
        /// Gets or sets the current iteration.
        /// </summary>
        public int Iteration
        {
            get => _iteration;
            set
            {
                this.RaiseAndSetIfChanged(ref _iteration, value);
                ChangeCurrentIteration();
            }
        }

        private int _depictionIndex;
        /// <summary>
        /// Gets or sets the population depiction currently seleccted.
        /// </summary>
        public int DepictionIndex
        {
            get => _depictionIndex;
            set
            {
                this.RaiseAndSetIfChanged(ref _depictionIndex, value);

                if (RunData != null)
                {
                    UpdatePopulations(_shownIterationData);
                }
            }
        }

        private int _bitDepictionIndex;
        /// <summary>
        /// Gets or sets the bit depiction currently seleccted.
        /// </summary>
        public int BitDepictionIndex
        {
            get => _bitDepictionIndex;
            set
            {
                this.RaiseAndSetIfChanged(ref _bitDepictionIndex, value);
                GlobalManager.Instance.IsBarCodeDepiction = value == 1;
            }
        }

        private int _modusIndex;
        /// <summary>
        /// Gets or sets the animation modus currently seleccted.
        /// </summary>
        public int ModusIndex
        {
            get => _modusIndex;
            set
            {
                this.RaiseAndSetIfChanged(ref _modusIndex, value);

                if (value != 0 && RunData != null)
                {
                    GlobalManager.Instance.NotifyFinishedIteration();
                }

                AnimationModus animationModus = ((AnimationModus[])Enum.GetValues(typeof(AnimationModus)))[_modusIndex];
                GlobalManager.Instance.AnimationModus = animationModus;
            }
        }

        /// <summary>
        /// Gets or sets the max iteration of the simulation.
        /// </summary>
        [Reactive]
        public int MaxIteration { get; set; }

        /// <summary>
        /// Gets or sets, if the simulation is automatically running.
        /// </summary>
        [Reactive]
        public bool Running { get; set; }


        /// <summary>
        /// Gets, if stepping forward is allowed.
        /// </summary>
        public bool RightButtonEnabled
        {
            get => !Running && RunData != null && !RunData.Iterations[Iteration].LastIteration
                && (GlobalManager.Instance.AnimationModus == AnimationModus.NONE
                        || !GlobalManager.Instance.IsAnimatingFOS);
        }

        /// <summary>
        /// Gets, if stepping backward is allowed.
        /// </summary>
        public bool LeftButtonEnabled
        {
            get => !Running && Iteration > 0
                 && (GlobalManager.Instance.AnimationModus == AnimationModus.NONE
                        || !GlobalManager.Instance.IsAnimatingFOS);
        }

        /// <summary>
        /// Gets, if saving is enabled.
        /// </summary>
        public bool IsSaveEnabled
        {
            get
            {
                return RunData != null && Saver.IsValidPath(RunData.FilePath);
            }
        }

        /// <summary>
        /// Gets, if the button to save the population depiction should be enabled.
        /// </summary>
        public bool IsScreenSaveEnabled
        {
            get
            {
                return RunData != null;
            }
        }


        /// <summary>
        /// Gets the tick spacing for the timeline.
        /// </summary>
        [Reactive]
        public int TickSpacing { get; private set; } = 1;

        /// <summary>
        /// Gets the rundata information of the current simulation.
        /// </summary>
        [Reactive]
        public RunData RunData { get; private set; }


        private PopulationDepictionViewModelBase[] _models = [
            new PopulationBlocksViewModel(),
            null, // Graph needs rundata
            new PopulationBarsViewModel(),
            null, // BoxPlot needs rundata
        ];

        /// <summary>
        /// Gets or sets the population depiction view model currently selected.
        /// </summary>
        public PopulationDepictionViewModelBase Model { get; set; }

        private bool _stopThread;

        private bool _excludeRecalculationInNextIterationChange;

        private IterationData _shownIterationData;

        /// <summary>
        /// Gets the message shown at the top of the window.
        /// </summary>
        [Reactive]
        public string Message { get; private set; }

        /// <summary>
        /// Gets, if the local search function should be shown.
        /// </summary>
        [Reactive]
        public bool ShowLocalSearchFunction { get; private set; }

        /// <summary>
        /// Gets, if the population size should be shown.
        /// </summary>
        [Reactive]
        public bool ShowPopulationSize { get; private set; }

        /// <summary>
        /// Gets, if the growth function should be shown.
        /// </summary>
        [Reactive]
        public bool ShowGrowthFunction { get; private set; }

        /// <summary>
        /// Creates a new instance of the main window view model.
        /// </summary>
        public MainWindowViewModel()
        {
            Thread playThread = new Thread(new ThreadStart(() =>
            {
                while (!_stopThread)
                {
                    if (Running && !GlobalManager.Instance.IsAnimatingDetails
                        && (GlobalManager.Instance.AnimationModus == AnimationModus.NONE
                        || !GlobalManager.Instance.IsAnimatingFOS))
                    {
                        long start = Stopwatch.GetTimestamp();
                        Dispatcher.UIThread.Invoke(StepForward);
                        long delay = Stopwatch.GetTimestamp() - start;

                        // Ensure that mouseclick is registered, when calculation takes long
                        Thread.Sleep(Math.Min((int)(delay / TimeSpan.TicksPerMillisecond), 2000));
                    }
                    RaiseButtonsChanged();
                    Thread.Sleep(100);
                }
            }));
            playThread.Start();
        }

        /// <summary>
        /// Forcefully stops the automatic simulation run.
        /// </summary>
        public void Stop()
        {
            _stopThread = true;
        }

        /// <summary>
        /// Selects the population container at the specified index.
        /// </summary>
        /// <param name="index">The index of the population to select</param>
        public void SelectPopulation(int index)
        {
            _models[DepictionIndex].SelectPopulation(index);
        }

        /// <summary>
        /// Updates the population depictions with new iteration data.
        /// </summary>
        /// <param name="iterationData"></param>
        public void UpdatePopulations(IterationData iterationData)
        {
            _shownIterationData = iterationData;
            if (_shownIterationData.LastIteration)
            {
                Running = false;
                Message = RunData.TerminationCriteria.GetTerminationString();
                RaiseButtonsChanged();
            }

            _models[DepictionIndex].SelectedPopulation = Model != null ? Model.SelectedPopulation : -1;
            _models[DepictionIndex].Update(_shownIterationData);
            Model = _models[DepictionIndex];
            this.RaisePropertyChanged(nameof(Model));
        }

        /// <summary>
        /// Changes the current iteration and all the visualisation data.
        /// </summary>
        public void ChangeCurrentIteration()
        {
            Message = "";
            GlobalManager.Instance.CurrentIteration = Iteration;

            // If iteration should be visualized (Globalmanager calls UpdatePopulations())
            if (ModusIndex == 0 && !_excludeRecalculationInNextIterationChange)
            {
                IList<IStateChange> result;
                if (Iteration > 0)
                {
                    result = RunData.Algorithm.CalculateIterationStateChanges(RunData.Iterations[Iteration - 1], RunData);
                }
                else
                {
                    result = RunData.Algorithm.CalculateIterationStateChanges(GlobalManager.Instance.InitialIteration(), RunData); ;
                }
                GlobalManager.Instance.StartIterationVisualization(result);
            }
            else if (ModusIndex != 0)
            {
                UpdatePopulations(RunData.Iterations[Iteration]);
            }
            _excludeRecalculationInNextIterationChange = false;

            RaiseButtonsChanged();
        }

        /// <summary>
        /// Toggles automatically running the simulation.
        /// </summary>
        public void Play()
        {
            Running = !Running;
            RaiseButtonsChanged();
        }

        /// <summary>
        /// Steps one iteration forward.
        /// </summary>
        public void StepForward()
        {
            GlobalManager.Instance.NotifyFinishedIteration();

            if (Iteration < RunData.Iterations.Count
                && RunData.Iterations.ElementAt(Iteration).LastIteration)
            {
                return;
            }
            if (Iteration == MaxIteration)
            {
                var result = RunData.Algorithm.CalculateIteration(RunData.Iterations[Iteration], RunData);
                RunData.Iterations.Add(result.Item1);

                if (ModusIndex == 0)
                {
                    // Needs to be set here because needed in visualisation start
                    GlobalManager.Instance.CurrentIteration = Iteration + 1;
                    GlobalManager.Instance.StartIterationVisualization(result.Item2);
                }

                MaxIteration++;
                _excludeRecalculationInNextIterationChange = true;


                CalculateTickSpacing();
            }
            Iteration++;
        }

        /// <summary>
        /// Steps one iteration back.
        /// </summary>
        public void StepBackward()
        {
            if (_iteration > 0)
            {
                GlobalManager.Instance.NotifyFinishedIteration();
                Iteration--;
            }
        }

        private void CalculateTickSpacing()
        {
            foreach (var t in TICK_STEPS)
            {
                if (MaxIteration > t) { TickSpacing = t / 10; break; }
            }
        }

        /// <summary>
        /// Opens the simulation creation window.
        /// </summary>
        public void New()
        {
            GlobalManager.Instance.OpenNewAlgorithmWindow();
        }


        /// <summary>
        /// Saves the current active simulation run to the disk, if it was already saved before.
        /// </summary>
        public void Save()
        {
            Saver.SaveData(RunData, RunData.FilePath!);
        }

        /// <summary>
        /// Saves the current active simulation run to the disk in the specified location.
        /// </summary>
        /// <param name="path">The location of where to save the file.</param>
        public void SaveAs(string path)
        {
            Saver.SaveData(RunData, path);

            RunData.FilePath = path;
            this.RaisePropertyChanged(nameof(IsSaveEnabled));
        }

        /// <summary>
        /// Loads a simulation run from the disk.
        /// </summary>
        /// <param name="path">The path of the file to load.</param>
        public void Load(string path)
        {
            GlobalManager.Instance.SetNewRunData(Loader.LoadData(path));
        }

        /// <summary>
        /// Sets the rundata of the currently running simulation.
        /// </summary>
        /// <param name="runData"></param>
        public void SetNewRunData(RunData runData)
        {
            RunData = runData;
            _models[1] = new PopulationGraphsViewModel(RunData);
            _models[3] = new PopulationBoxPlotViewModel(RunData);

            if (runData.Iterations.Count > 0)
            {
                MaxIteration = runData.Iterations.Count - 1;
            }
            else
            {
                var result = RunData.Algorithm.CalculateIteration(
                GlobalManager.Instance.InitialIteration(),
                RunData);

                RunData.Iterations.Add(result.Item1);

                if (ModusIndex == 0)
                {
                    GlobalManager.Instance.StartIterationVisualization(result.Item2);
                }
                else
                {
                    UpdatePopulations(RunData.Iterations[0]);
                }

                _excludeRecalculationInNextIterationChange = true;
                MaxIteration = 0;
            }
            Iteration = 0;
            RaiseButtonsChanged();
            this.RaisePropertyChanged(nameof(IsScreenSaveEnabled));
        }

        private void RaiseButtonsChanged()
        {
            this.RaisePropertyChanged(nameof(RightButtonEnabled));
            this.RaisePropertyChanged(nameof(LeftButtonEnabled));
        }
    }
}
