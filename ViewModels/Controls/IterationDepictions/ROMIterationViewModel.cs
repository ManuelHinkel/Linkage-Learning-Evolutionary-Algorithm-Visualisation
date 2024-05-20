using Avalonia.Threading;
using LLEAV.Models;

using LLEAV.Models.Algorithms.ROM.StateChange;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        ];

        protected override IList<string> animationProperties { get; } = [
            "IsMerging",
            "IsFitnessIncreasing",
            "IsFitnessDecreasing",
        ];

        private IList<IROMStateChange> _stateChanges { get; set; }
        private ROMVisualisationData _visualisationData = new ROMVisualisationData();

        private Tuple<IterationData, ROMVisualisationData, IList<string>>[] _checkpoints;

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
            get => IsMergingRunning || _isApplyingFitnessIncreaseRunning || _isApplyingFitnessDecreaseRunning;
        }

        public ROMIterationViewModel(List<IROMStateChange> stateChanges, IterationData workingData): base()
        {
            _stateChanges = stateChanges;
            WorkingData = workingData.Clone();
            MaxStateChange = _stateChanges.Count - 1;

            _checkpoints = new Tuple<IterationData, ROMVisualisationData, IList<string>>[(int)Math.Ceiling(MaxStateChange / (float)CHECKPOINT_SPACING)];


            Thread calculationThread = new Thread(new ThreadStart(() => {
                CalculateCheckpoints(workingData.Clone());
            }));
            calculationThread.Start();


            _currentStateChange = -1;

            if (_stateChanges.Count > 0)
            {
                GoToStateChange(0);
            }

            CalculateTickSpacing();
        }

        private void CalculateCheckpoints(IterationData baseData)
        {
            IterationData workingIterationData = baseData;
            ROMVisualisationData workingVisualisationData = new ROMVisualisationData();

            IList<string> messages = new List<string>();

            for (int i = 0; i < MaxStateChange; i++)
            {
                var res = _stateChanges[i].Apply(workingIterationData, workingVisualisationData, true);

                messages.Add(res.Item2);
                if (i % CHECKPOINT_SPACING == 0)
                {
                    _checkpoints[i / CHECKPOINT_SPACING] = new Tuple<IterationData, ROMVisualisationData, IList<string>>(
                        workingIterationData.Clone(),
                        (ROMVisualisationData)workingVisualisationData.Clone(),
                        new List<string>(messages)
                        );
                }
            }
        }

        protected override void RaiseChanged(IList<string> properties)
        {
            foreach (var property in properties)
            {
                // Start Merging Animation
                if (property.Equals(nameof(IsMerging)) && _visualisationData.IsMerging)
                {
                    IsMergingRunning = true;
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
            RaiseButtonsChanged();
        }

        private void VisualizeCheckpoint(Tuple<IterationData, ROMVisualisationData, IList<string>> checkPoint)
        {
            MessageBox.Clear();
            foreach (string m in checkPoint.Item3)
            {
                MessageBox.Insert(0, m);
            }

            GlobalManager.Instance.SelectPopulation(checkPoint.Item1, 0);
            if (checkPoint.Item2.ActiveCluster != null)
            {
                GlobalManager.Instance.SelectCluster(0, checkPoint.Item2.ActiveCluster);
            }

            WorkingData = checkPoint.Item1.Clone();
            _visualisationData = (ROMVisualisationData)checkPoint.Item2.Clone();
            
            RaiseChanged(VISUALISATION_PROPERTIES);
        }

        protected override void GoToStateChange(int index)
        {
            if (_currentStateChange == index) return;

            int checkpointIndex = _currentStateChange / CHECKPOINT_SPACING;
            int targetCheckpointIndex = index / CHECKPOINT_SPACING;

            if ((index != _currentStateChange+1) && (checkpointIndex != targetCheckpointIndex || index < _currentStateChange))
            {
                // Wait until checkpoint calculated
                while (_checkpoints[targetCheckpointIndex] == null)
                {
                    Thread.Sleep(100);
                }
                VisualizeCheckpoint(_checkpoints[targetCheckpointIndex]);
                _currentStateChange = targetCheckpointIndex * CHECKPOINT_SPACING;
            }

            ISet<string> propertiesChanged = new HashSet<string>();
            while (_currentStateChange < index)
            {
                _currentStateChange++;
                var changed = _stateChanges[_currentStateChange].Apply(WorkingData, _visualisationData);
                MessageBox.Insert(0, changed.Item2);
                CombineProperties(changed.Item1, propertiesChanged);
            }
            RaiseChanged(propertiesChanged.ToList());
        }
    }
}
