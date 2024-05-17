﻿using Avalonia.Threading;
using LiveChartsCore.Kernel;
using LLEAV.Models;
using LLEAV.Models.Algorithms;
using LLEAV.Models.Algorithms.MIP.StateChange;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;


namespace LLEAV.ViewModels.Controls.IterationDepictions
{
    public class MIPIterationViewModel : IterationDepictionViewModelBase
    {
        private static readonly IList<string> VISUALISATION_PROPERTIES = [
            "CurrentSolution",
            "CurrentDonor",
            "Merged",
            "Solutions",
            "Donors",
            "IsMerging",
            "IsApplyingCrossover",
        ];

        private IList<IMIPStateChange> _stateChanges;
        private MIPVisualisationData _visualisationData = new MIPVisualisationData();

        public IList<Tuple<SolutionWrapper,SolutionWrapper>> Solutions
        {
            get => _visualisationData.Solutions;
        }

        public IList<SolutionWrapper> Donors
        {
            get => _visualisationData.Donors;
        }

        public SolutionWrapper? CurrentSolution
        {
            get
            {
                if (_visualisationData.CurrentSolution != null)
                {
                    var wrapper = new SolutionWrapper(_visualisationData.CurrentSolution);
                    if (_visualisationData.ActiveCluster != null)
                    {
                        wrapper.MarkCluster(!_visualisationData.ActiveCluster, "#0000ff");
                    }
                    return wrapper;
                }
                return null;
            }
        }

        public SolutionWrapper? CurrentDonor
        {
            get 
            { 
                if (_visualisationData.CurrentDonor != null)
                {
                    var wrapper = new SolutionWrapper(_visualisationData.CurrentDonor);
                    wrapper.MarkCluster(_visualisationData.ActiveCluster, "#00ff00");
                    return wrapper;
                }
                return null; 
            }
        }

        public SolutionWrapper? Merged
        {
            get 
            {
                if (_visualisationData.Merged != null)
                {
                    var wrapper = new SolutionWrapper(_visualisationData.Merged);
                    if (_visualisationData.ActiveCluster != null)
                    {
                        wrapper.MarkCluster(_visualisationData.ActiveCluster, "#00ff00");
                        wrapper.MarkCluster(!_visualisationData.ActiveCluster, "#0000ff");
                    }
                    return wrapper;
                }
                return null;
            }
        }

        public bool IsMerging
        {
            get { return _visualisationData.IsMerging; }
        }

        public bool IsApplyingCrossover
        {
            get { return _visualisationData.IsApplyingCrossover; }
        }

        public MIPIterationViewModel(IList<IMIPStateChange> stateChanges, IterationData workingData)
        {
            _stateChanges = stateChanges;
            BaseData = workingData;
            WorkingData = BaseData.Clone();
            MaxStateChange = _stateChanges.Count - 1;


            if (_stateChanges.Count > 0)
            {
                _stateChanges[0].Apply(WorkingData, _visualisationData);
            }

            CalculateTickSpacing();
        }

        public override void StepForward()
        {
            if (CurrentStateChange < MaxStateChange)
            {
                CurrentStateChange++;
            } 
            if (CurrentStateChange == MaxStateChange)
            {
                GlobalManager.Instance.NotifyFinishedIteration();
                Running = false;
            }
            RaiseButtonsChanged();
        }

        public override void StepBackward()
        {
            if (CurrentStateChange > 0)
            {
                CurrentStateChange--;
            } 
            RaiseButtonsChanged();
        }

        protected override void RaiseChanged(IList<string> properties)
        {
            foreach (var property in properties)
            {
                // Start Merging Animation
                if (property.Equals("IsMerging") && _visualisationData.IsMerging)
                {
                    IsMergingRunning = true;
                    RaiseButtonsChanged();
                    Thread t = new Thread(new ThreadStart(() => {
                        Thread.Sleep(GlobalManager.ANIMATION_TIME);
                        Dispatcher.UIThread.Invoke(() => {
                            _visualisationData.IsMerging = false;
                            this.RaisePropertyChanged(nameof(IsMerging));

                            IsMergingRunning = false;
                            RaiseButtonsChanged();
                        });
                    }));
                    t.Start();
                }
                else if (property.Equals("IsApplyingCrossover") && _visualisationData.IsApplyingCrossover)
                {
                    IsApplyingCrossoverRunning = true;
                    RaiseButtonsChanged();
                    Thread t = new Thread(new ThreadStart(() => {
                        Thread.Sleep(GlobalManager.ANIMATION_TIME);
                        Dispatcher.UIThread.Invoke(() => {
                            _visualisationData.IsApplyingCrossover = false;
                            this.RaisePropertyChanged(nameof(IsApplyingCrossover));

                            IsApplyingCrossoverRunning = false;
                            RaiseButtonsChanged();
                        });
                    }));
                    t.Start();
                }


                this.RaisePropertyChanged(property);
            }
        }

        protected override void GoToStateChange(int index)
        {
            if (_currentStateChange == index) return;
            // Go forward
            if (index > _currentStateChange)
            {
                ISet<string> propertiesChanged = new HashSet<string>();
                while (_currentStateChange < index)
                {
                    _currentStateChange++;
                    var changed = _stateChanges[_currentStateChange].Apply(WorkingData, _visualisationData);
                    MessageBox.Insert(0, changed.Item2);
                    propertiesChanged.UnionWith(changed.Item1);
                }
                RaiseChanged(propertiesChanged.ToList());

            } 
            // Go forward from base
            else 
            {
                GlobalManager.Instance.UpdatePopulationWindowIfOpen(new Population(0));
                GlobalManager.Instance.UpdateMainWindow(BaseData);
                WorkingData = BaseData.Clone();
                _visualisationData = new MIPVisualisationData();
                MessageBox.Clear();
                _currentStateChange = 0;

                while (_currentStateChange < index)
                {
                    _currentStateChange++;
                    var changed = _stateChanges[_currentStateChange].Apply(WorkingData, _visualisationData);
                    MessageBox.Insert(0, changed.Item2);
                }
                RaiseChanged(VISUALISATION_PROPERTIES);
            }
        }

        
    }
}
