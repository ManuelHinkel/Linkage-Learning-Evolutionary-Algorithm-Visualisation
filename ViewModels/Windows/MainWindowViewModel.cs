using Avalonia.Threading;
using LLEAV.Models;
using LLEAV.Models.Algorithms;
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

                if (_runData != null)
                {
                    UpdatePopulations(_runData.Iterations[Iteration]);
                }
            }
        }

        private int _modusIndex;
        public int ModusIndex 
        {
            get => _modusIndex;
            set
            {
                this.RaiseAndSetIfChanged(ref _modusIndex, value);

                if (value != 0 && _runData != null)
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
            get => !Running && _runData != null && !_runData.Iterations[Iteration].LastIteration;
        }

        public bool LeftButtonEnabled
        {
            get => !Running && Iteration > 0;
        }

        [Reactive]
        public int TickSpacing { get; private set; } = 1;

        public PopulationBlocksViewModel BlockModel { get; set; } = new PopulationBlocksViewModel();
        public PopulationGraphsViewModel GraphModel { get; set; }
        public PopulationBarsViewModel BarModel { get; set; } = new PopulationBarsViewModel();

        private RunData _runData;

        private bool _stopThread;

        private bool _excludeRecalculationInNextIterationChange;
        public MainWindowViewModel()
        {
            Thread playThread = new Thread(new ThreadStart(() => {
                while (!_stopThread)
                {
                    if (Running && !GlobalManager.Instance.IsAnimatingDetails 
                        && (GlobalManager.Instance.AnimationModus == AnimationModus.NONE 
                        || !GlobalManager.Instance.IsAnimatingFOS))
                    {
                        Dispatcher.UIThread.Invoke(StepForward);
                    }
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
            switch (DepictionIndex)
            {
                case 0:
                    BlockModel.SelectPopulation(index);
                    break;
                case 1:
                    GraphModel.SelectPopulation(index);
                    break;
                default:
                    BarModel.SelectPopulation(index);
                    break;
            }
        }

        public void UpdatePopulations(IterationData iterationData)
        {
            if (iterationData.LastIteration)
            {
                Running = false;
                RaiseButtonsChanged();
            }

            switch (DepictionIndex)
            {
                case 0:
                    BlockModel.Update(iterationData);
                    this.RaisePropertyChanged(nameof(BlockModel));
                    break;
                case 1:
                    GraphModel.Update(iterationData);
                    this.RaisePropertyChanged(nameof(GraphModel));
                    break;
                default:
                    BarModel.Update(iterationData);
                    this.RaisePropertyChanged(nameof(BarModel));
                    break;
            }
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
                    result = _runData.Algorithm.CalculateIterationStateChanges(_runData.Iterations[Iteration - 1], _runData);
                } else
                {
                    result = _runData.Algorithm.CalculateIterationStateChanges(GlobalManager.Instance.InitialIteration(), _runData); ;
                }
                GlobalManager.Instance.StartIterationVisualization(result);
            }
            else if (ModusIndex != 0)
            {
                UpdatePopulations(_runData.Iterations[Iteration]);
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

            if (Iteration < _runData.Iterations.Count
                && _runData.Iterations.ElementAt(Iteration).LastIteration) return;
            if (Iteration == MaxIteration)
            {
                var result = _runData.Algorithm.CalculateIteration(_runData.Iterations[Iteration], _runData);
                 _runData.Iterations.Add(result.Item1);

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

        }

        public void SaveAs(string path)
        {

        }

        public void Load(string path)
        {

        }

        public void SetNewRunData(RunData runData)
        {
            _runData = runData;
            GraphModel = new PopulationGraphsViewModel(_runData);

            var result = _runData.Algorithm.CalculateIteration(
                GlobalManager.Instance.InitialIteration(),
                _runData);
            
            _runData.Iterations.Add(result.Item1);

            if (ModusIndex == 0)
            {
                GlobalManager.Instance.StartIterationVisualization(result.Item2);
            } else
            {
                UpdatePopulations(_runData.Iterations[0]);
            }

            _excludeRecalculationInNextIterationChange = true;
            Iteration = 0;
            MaxIteration = 0;

            RaiseButtonsChanged();
        }

        private void RaiseButtonsChanged()
        {
            this.RaisePropertyChanged(nameof(RightButtonEnabled));
            this.RaisePropertyChanged(nameof(LeftButtonEnabled));
        }
    }
}
