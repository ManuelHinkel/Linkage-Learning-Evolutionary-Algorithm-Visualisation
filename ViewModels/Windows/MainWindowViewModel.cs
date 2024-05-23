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
        private int _iteration;
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


        [Reactive]
        public int MaxIteration { get; set; }

        [Reactive]
        public bool Running { get; set; }


        public bool RightButtonEnabled
        {
            get => !Running && RunData != null && !RunData.Iterations[Iteration].LastIteration
                && (GlobalManager.Instance.AnimationModus == AnimationModus.NONE
                        || !GlobalManager.Instance.IsAnimatingFOS);
        }

        public bool LeftButtonEnabled
        {
            get => !Running && Iteration > 0
                 && (GlobalManager.Instance.AnimationModus == AnimationModus.NONE
                        || !GlobalManager.Instance.IsAnimatingFOS);
        }

        public bool IsSaveEnabled
        {
            get
            {
                return RunData != null && Saver.IsValidPath(RunData.FilePath);
            }
        }

        [Reactive]
        public int TickSpacing { get; private set; } = 1;

        [Reactive]
        public RunData RunData { get; private set; }


        private PopulationDepictionViewModelBase[] _models = [
            new PopulationBlocksViewModel(),
            null, // Graph needs rundata
            new PopulationBarsViewModel(),
            null, // BoxPlot needs rundata
        ];


        public PopulationDepictionViewModelBase Model { get; set; }

        private bool _stopThread;

        private bool _excludeRecalculationInNextIterationChange;

        private IterationData _shownIterationData;
        public MainWindowViewModel()
        {
            Thread playThread = new Thread(new ThreadStart(async () => {
                while (!_stopThread)
                {
                    if (Running && !GlobalManager.Instance.IsAnimatingDetails 
                        && (GlobalManager.Instance.AnimationModus == AnimationModus.NONE 
                        || !GlobalManager.Instance.IsAnimatingFOS))
                    {
                        long start = Stopwatch.GetTimestamp();
                        await Dispatcher.UIThread.InvokeAsync(StepForward);
                        long delay = Stopwatch.GetTimestamp() - start;

                        // Ensure that mouseclick is registered, when calculation takes long
                        Thread.Sleep((int)(delay / TimeSpan.TicksPerMillisecond));
                    }
                    RaiseButtonsChanged();
                    Thread.Sleep(100);
                }
            }));
            playThread.Start();
        }

        public void Stop()
        {
            _stopThread = true;
        }

        public void SelectPopulation(int index)
        {
            _models[DepictionIndex].SelectPopulation(index);
        }

        public void UpdatePopulations(IterationData iterationData)
        {
            _shownIterationData = iterationData;
            if (_shownIterationData.LastIteration)
            {
                Running = false;
                RaiseButtonsChanged();
            }

            _models[DepictionIndex].SelectedPopulation = Model != null ? Model.SelectedPopulation : -1;
            _models[DepictionIndex].Update(_shownIterationData);
            Model = _models[DepictionIndex];
            this.RaisePropertyChanged(nameof(Model));
        }

        public void ChangeCurrentIteration()
        {
            GlobalManager.Instance.CurrentIteration = Iteration;

            // If iteration should be visualized (Globalmanager calls UpdatePopulations())
            if (ModusIndex == 0 && !_excludeRecalculationInNextIterationChange)
            {
                IList<IStateChange> result;
                if (Iteration > 0)
                {
                    result = RunData.Algorithm.CalculateIterationStateChanges(RunData.Iterations[Iteration - 1], RunData);
                } else
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

        public void Play()
        {
            Running = !Running;
            RaiseButtonsChanged();
        }

        public void StepForward()
        {
            GlobalManager.Instance.NotifyFinishedIteration();

            if (Iteration < RunData.Iterations.Count
                && RunData.Iterations.ElementAt(Iteration).LastIteration) return;
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

        private void CalculateTickSpacing()
        {
            if (MaxIteration > 500)
            {
                TickSpacing = 50;
            }
            else if (MaxIteration > 250)
            {
                TickSpacing = 20;
            }
            else if (MaxIteration > 100)
            {
                TickSpacing = 10;
            }
            else if (MaxIteration > 50)
            {
                TickSpacing = 5;
            }
        }

        public void StepBackward()
        {
            if (_iteration > 0)
            {
                GlobalManager.Instance.NotifyFinishedIteration();
                Iteration--;
            } 
        }

        public void New()
        {
            GlobalManager.Instance.OpenNewAlgorithmWindow();
        }


        public void Save()
        {
            Saver.SaveData(RunData, RunData.FilePath!);
        }

        public void SaveAs(string path)
        {
            Saver.SaveData(RunData, path);

            RunData.FilePath = path;
            this.RaisePropertyChanged(nameof(IsSaveEnabled));
        }

        public void Load(string path)
        {
            GlobalManager.Instance.SetNewRunData(Loader.LoadData(path));
        }

        public void SetNewRunData(RunData runData)
        {
            RunData = runData;
            _models[1] = new PopulationGraphsViewModel(RunData);
            _models[3] = new PopulationBoxPlotViewModel(RunData);

            if (runData.Iterations.Count > 0)
            {
                MaxIteration = runData.Iterations.Count - 1;
            } else
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
        }

        private void RaiseButtonsChanged()
        {
            this.RaisePropertyChanged(nameof(RightButtonEnabled));
            this.RaisePropertyChanged(nameof(LeftButtonEnabled));
        }
    }
}
