using Avalonia.Threading;
using LLEAV.Models;

using LLEAV.Models.Algorithms.ROM.StateChange;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LLEAV.ViewModels.Controls.IterationDepictions
{
    public class ROMIterationViewModel : IterationDepictionViewModelBase
    {
        private static readonly IList<string> VISUALISATION_PROPERTIES = [
            "CurrentSolution1",
            "CurrentSolution2",
            "CurrentDonor1",
            "CurrentDonor2",
            "Solutions",
            "IsMerging",
            "IsFitnessIncreasing",
            "IsFitnessDecreasing",
        ];

        private IList<IROMStateChange> _stateChanges { get; set; }
        private ROMVisualisationData _visualisationData = new ROMVisualisationData();



        public IList<SolutionWrapper> Solutions
        {
            get => _visualisationData.Solutions;
        }

        public IList<SolutionWrapper> NextIteration
        {
            get => _visualisationData.NextIteration;
        }

        public SolutionWrapper CurrentSolution1
        {
            get
            {
                if (_visualisationData.CurrentSolution1 != null)
                {
                    var wrapper = new SolutionWrapper(_visualisationData.CurrentSolution1);
                    if (_visualisationData.ActiveCluster != null)
                    {
                        wrapper.MarkCluster(!_visualisationData.ActiveCluster, CLUSTER_HIGHLIGHT_COLOR_1);
                    }
                    return wrapper;
                }
                return null;
            }
        }

        public SolutionWrapper CurrentSolution2
        {
            get
            {
                if (_visualisationData.CurrentSolution1 != null)
                {
                    var wrapper = new SolutionWrapper(_visualisationData.CurrentSolution1);
                    if (_visualisationData.ActiveCluster != null)
                    {
                        wrapper.MarkCluster(!_visualisationData.ActiveCluster, CLUSTER_HIGHLIGHT_COLOR_1);
                    }
                    return wrapper;
                }
                return null;
            }
        }

        public SolutionWrapper CurrentDonor1
        {
            get
            {
                if (_visualisationData.CurrentDonor1 != null)
                {
                    var wrapper = new SolutionWrapper(_visualisationData.CurrentDonor1);
                    wrapper.MarkCluster(_visualisationData.ActiveCluster, CLUSTER_HIGHLIGHT_COLOR_2);
                    return wrapper;
                }
                return null;
            }
        }

        public SolutionWrapper CurrentDonor2
        {
            get
            {
                if (_visualisationData.CurrentDonor1 != null)
                {
                    var wrapper = new SolutionWrapper(_visualisationData.CurrentDonor1);
                    wrapper.MarkCluster(_visualisationData.ActiveCluster, CLUSTER_HIGHLIGHT_COLOR_2);
                    return wrapper;
                }
                return null;
            }
        }

        public bool IsMerging
        {
            get { return _visualisationData.IsMerging; }
        }

        public bool IsFitnessIncreasing
        {
            get { return _visualisationData.IsFitnessIncreasing; }
        }

        public bool IsFitnessDecreasing
        {
            get { return _visualisationData.IsFitnessDecreasing; }
        }

        private bool _isApplyingFitnessIncreaseRunning { get; set; }

        private bool _isApplyingFitnessDecreaseRunning { get; set; }

        protected override bool isAnimating
        {
            get => IsMerging || _isApplyingFitnessIncreaseRunning || _isApplyingFitnessDecreaseRunning;
        }

        public ROMIterationViewModel(List<IROMStateChange> stateChanges, IterationData workingData)
        {
            _stateChanges = stateChanges;
            BaseData = workingData;
            WorkingData = BaseData.Clone();
            MaxStateChange = _stateChanges.Count - 1;
            _currentStateChange = -1;

            if (_stateChanges.Count > 0)
            {
                GoToStateChange(0);
            }

            CalculateTickSpacing();
        }

        protected override void RaiseChanged(IList<string> properties)
        {
            foreach (var property in properties)
            {
                // Start Merging Animation
                if (property.Equals(nameof(IsMerging)) && _visualisationData.IsMerging)
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
                else if (property.Equals(nameof(IsFitnessIncreasing)) && _visualisationData.IsFitnessIncreasing)
                {
                    _isApplyingFitnessIncreaseRunning = true;
                    RaiseButtonsChanged();
                    Thread t = new Thread(new ThreadStart(() => {
                        Thread.Sleep(GlobalManager.ANIMATION_TIME);
                        Dispatcher.UIThread.Invoke(() => {
                            _visualisationData.IsFitnessIncreasing = false;
                            this.RaisePropertyChanged(nameof(IsFitnessIncreasing));

                            _isApplyingFitnessIncreaseRunning = false;
                            RaiseButtonsChanged();
                        });
                    }));
                    t.Start();
                }
                else if (property.Equals(nameof(IsFitnessDecreasing)) && _visualisationData.IsFitnessDecreasing)
                {
                    _isApplyingFitnessDecreaseRunning = true;
                    RaiseButtonsChanged();
                    Thread t = new Thread(new ThreadStart(() => {
                        Thread.Sleep(GlobalManager.ANIMATION_TIME);
                        Dispatcher.UIThread.Invoke(() => {
                            _visualisationData.IsFitnessDecreasing = false;
                            this.RaisePropertyChanged(nameof(IsFitnessDecreasing));

                            _isApplyingFitnessDecreaseRunning = false;
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
                _visualisationData = new ROMVisualisationData();
                MessageBox.Clear();
                _currentStateChange = -1;

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
