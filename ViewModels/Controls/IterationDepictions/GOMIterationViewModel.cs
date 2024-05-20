using Avalonia.Threading;
using LLEAV.Models;
using LLEAV.Models.Algorithms;
using LLEAV.Models.Algorithms.GOM.StateChange;
using LLEAV.Models.Algorithms.MIP.StateChange;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace LLEAV.ViewModels.Controls.IterationDepictions
{
    public class GOMIterationViewModel : IterationDepictionViewModelBase
    {
        private static readonly IList<string> VISUALISATION_PROPERTIES = [
            "CurrentSolution",
            "CurrentDonor",
            "Merged",
            "Solutions",
        ];

        protected override IList<string> animationProperties { get; } = [
           "IsMerging",
            "IsApplyingCrossover",
        ];


        private IList<IGOMStateChange> _stateChanges { get; set; }
        private GOMVisualisationData _visualisationData = new GOMVisualisationData();

        private Tuple<IterationData, GOMVisualisationData, IList<string>>[] _checkpoints;
        public IList<SolutionWrapper> Solutions
        {
            get => _visualisationData.Solutions;
        }

        public IList<SolutionWrapper> NextIteration
        {
            get => _visualisationData.NextIteration;
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
                        wrapper.MarkCluster(!_visualisationData.ActiveCluster, CLUSTER_HIGHLIGHT_COLOR_1);
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
                    wrapper.MarkCluster(_visualisationData.ActiveCluster, CLUSTER_HIGHLIGHT_COLOR_2);
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
                        wrapper.MarkCluster(_visualisationData.ActiveCluster, CLUSTER_HIGHLIGHT_COLOR_2);
                        wrapper.MarkCluster(!_visualisationData.ActiveCluster, CLUSTER_HIGHLIGHT_COLOR_1);
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

        private bool _isApplyingCrossoverRunning { get; set; }

        protected override bool isAnimating
        {
            get => IsMergingRunning || _isApplyingCrossoverRunning;
        }

        public GOMIterationViewModel(List<IGOMStateChange> stateChanges, IterationData workingData): base()
        {
            _stateChanges = stateChanges;
            WorkingData = workingData.Clone();
            MaxStateChange = _stateChanges.Count - 1;

            _checkpoints = new Tuple<IterationData, GOMVisualisationData, IList<string>>[(int)Math.Ceiling(MaxStateChange / (float)CHECKPOINT_SPACING)];


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
            GOMVisualisationData workingVisualisationData = new GOMVisualisationData();

            IList<string> messages = new List<string>();

            for (int i = 0; i < MaxStateChange; i++)
            {
                var res = _stateChanges[i].Apply(workingIterationData, workingVisualisationData, true);

                messages.Add(res.Item2);
                if (i % CHECKPOINT_SPACING == 0)
                {
                    _checkpoints[i / CHECKPOINT_SPACING] = new Tuple<IterationData, GOMVisualisationData, IList<string>>(
                        workingIterationData.Clone(),
                        (GOMVisualisationData)workingVisualisationData.Clone(),
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
                else if (property.Equals(nameof(IsApplyingCrossover)) && _visualisationData.IsApplyingCrossover)
                {
                    _isApplyingCrossoverRunning = true;
                    Thread t = new Thread(new ThreadStart(() => {
                        Thread.Sleep(GlobalManager.ANIMATION_TIME);
                        Dispatcher.UIThread.Invoke(() => {
                            _visualisationData.IsApplyingCrossover = false;
                            this.RaisePropertyChanged(nameof(IsApplyingCrossover));

                            _isApplyingCrossoverRunning = false;
                            RaiseButtonsChanged();
                        });
                    }));
                    t.Start();
                }

                this.RaisePropertyChanged(property);
            }
            RaiseButtonsChanged();
        }

        private void VisualizeCheckpoint(Tuple<IterationData, GOMVisualisationData, IList<string>> checkPoint)
        {
            MessageBox.Clear();
            foreach(string m in checkPoint.Item3) 
            {
                MessageBox.Insert(0, m);
            }

            GlobalManager.Instance.SelectPopulation(checkPoint.Item1, 0);
            if (checkPoint.Item2.ActiveCluster != null)
            {
                GlobalManager.Instance.SelectCluster(0, checkPoint.Item2.ActiveCluster);
            }

            WorkingData = checkPoint.Item1.Clone();
            _visualisationData = (GOMVisualisationData)checkPoint.Item2.Clone();

            RaiseChanged(VISUALISATION_PROPERTIES);
        }

        protected override void GoToStateChange(int index)
        {
            if (_currentStateChange == index) return;

            int checkpointIndex = _currentStateChange / CHECKPOINT_SPACING;
            int targetCheckpointIndex = index / CHECKPOINT_SPACING;

            if ((index != _currentStateChange + 1) && (checkpointIndex != targetCheckpointIndex || index < _currentStateChange))
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
