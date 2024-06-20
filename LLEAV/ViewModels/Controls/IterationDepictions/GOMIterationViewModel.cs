using Avalonia.Threading;
using LLEAV.Models;
using LLEAV.Models.Algorithms.GOM.StateChange;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace LLEAV.ViewModels.Controls.IterationDepictions
{
    /// <summary>
    /// ViewModel for managing the depiction of iterations in a GOMEA visualization.
    /// </summary>
    public class GOMIterationViewModel : IterationDepictionViewModelBase
    {
        private static readonly IList<string> VISUALISATION_PROPERTIES = [
            "CurrentSolution",
            "CurrentDonor",
            "Merged",
            "Solutions",
            "NextIteration",
        ];

        protected override IList<string> animationProperties { get; } = [
           "IsMerging",
            "IsApplyingCrossover",
        ];


        private IList<IGOMStateChange> _stateChanges { get; set; }
        private GOMVisualisationData _visualisationData = new GOMVisualisationData();

        private Tuple<IterationData, GOMVisualisationData, IList<Message>>[] _checkpoints;


        private bool _stopAddSolution;
        private bool _addSolutionRunning;

        /// <summary>
        /// Gets the list of solutions in teh current iteration.
        /// </summary>
        public ObservableCollection<SolutionWrapper> Solutions { get; } = [];


        private bool _stopAddNextIteration;
        private bool _addNextIterationRunning;

        /// <summary>
        /// Gets the list of solutions in the next iteration.
        /// </summary>
        public ObservableCollection<SolutionWrapper> NextIteration { get; private set; } = [];

        /// <summary>
        /// Gets the wrapper for the solution.
        /// </summary>
        public SolutionWrapper? CurrentSolution
        {
            get
            {
                if (_visualisationData.CurrentSolution != null)
                {
                    var wrapper = new SolutionWrapper(_visualisationData.CurrentSolution);
                    if (_visualisationData.ActiveCluster != null)
                    {
                        if (GlobalManager.Instance.IsBarCodeDepiction)
                        {
                            wrapper.MarkCluster(GlobalManager.CLUSTER_HIGHLIGHT_COLOR_1_ACTIVE, GlobalManager.CLUSTER_HIGHLIGHT_COLOR_1_INACTIVE,
                                !_visualisationData.ActiveCluster);
                        }
                        else
                        {
                            wrapper.MarkCluster(!_visualisationData.ActiveCluster, GlobalManager.CLUSTER_HIGHLIGHT_COLOR_1_ACTIVE);
                        }
                    }
                    return wrapper;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the wrapper for the donor.
        /// </summary>
        public SolutionWrapper? CurrentDonor
        {
            get
            {
                if (_visualisationData.CurrentDonor != null)
                {
                    var wrapper = new SolutionWrapper(_visualisationData.CurrentDonor);
                    if (_visualisationData.ActiveCluster != null)
                    {
                        if (GlobalManager.Instance.IsBarCodeDepiction)
                        {
                            wrapper.MarkCluster(GlobalManager.CLUSTER_HIGHLIGHT_COLOR_2_ACTIVE, GlobalManager.CLUSTER_HIGHLIGHT_COLOR_2_INACTIVE, _visualisationData.ActiveCluster);
                        }
                        else
                        {
                            wrapper.MarkCluster(_visualisationData.ActiveCluster, GlobalManager.CLUSTER_HIGHLIGHT_COLOR_2_ACTIVE);
                        }
                    }
                    return wrapper;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the wrapper for the merged solution.
        /// </summary>
        public SolutionWrapper? Merged
        {
            get
            {
                if (_visualisationData.Merged != null)
                {
                    var wrapper = new SolutionWrapper(_visualisationData.Merged);
                    if (_visualisationData.ActiveCluster != null)
                    {
                        if (GlobalManager.Instance.IsBarCodeDepiction)
                        {
                            wrapper.MarkCluster(GlobalManager.CLUSTER_HIGHLIGHT_COLOR_2_ACTIVE, GlobalManager.CLUSTER_HIGHLIGHT_COLOR_2_INACTIVE,
                                _visualisationData.ActiveCluster);
                            wrapper.MarkCluster(GlobalManager.CLUSTER_HIGHLIGHT_COLOR_1_ACTIVE, GlobalManager.CLUSTER_HIGHLIGHT_COLOR_1_INACTIVE,
                                !_visualisationData.ActiveCluster);
                        }
                        else
                        {
                            wrapper.MarkCluster(_visualisationData.ActiveCluster, GlobalManager.CLUSTER_HIGHLIGHT_COLOR_2_ACTIVE);
                            wrapper.MarkCluster(!_visualisationData.ActiveCluster, GlobalManager.CLUSTER_HIGHLIGHT_COLOR_1_ACTIVE);
                        }
                    }
                    return wrapper;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the wrapper for the animated solution.
        /// </summary>
        public SolutionWrapper? CurrentSolutionAnimated
        {
            get
            {
                if (_visualisationData.CurrentSolution != null)
                {
                    var wrapper = new SolutionWrapper(_visualisationData.CurrentSolution);
                    if (_visualisationData.ActiveCluster != null)
                    {
                        if (GlobalManager.Instance.IsBarCodeDepiction)
                        {
                            wrapper.MarkCluster(GlobalManager.CLUSTER_HIGHLIGHT_COLOR_1_ACTIVE, GlobalManager.CLUSTER_HIGHLIGHT_COLOR_1_INACTIVE,
                                !_visualisationData.ActiveCluster);
                        }
                        else
                        {
                            wrapper.MarkCluster(!_visualisationData.ActiveCluster, GlobalManager.CLUSTER_HIGHLIGHT_COLOR_1_ACTIVE);
                        }
                        wrapper.MarkCluster(_visualisationData.ActiveCluster, "#00000000");
                    }
                    return wrapper;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the wrapper for the animated donor.
        /// </summary>
        public SolutionWrapper? CurrentDonorAnimated
        {
            get
            {
                if (_visualisationData.CurrentDonor != null)
                {
                    var wrapper = new SolutionWrapper(_visualisationData.CurrentDonor);
                    if (_visualisationData.ActiveCluster != null)
                    {
                        if (GlobalManager.Instance.IsBarCodeDepiction)
                        {
                            wrapper.MarkCluster(GlobalManager.CLUSTER_HIGHLIGHT_COLOR_2_ACTIVE, GlobalManager.CLUSTER_HIGHLIGHT_COLOR_2_INACTIVE, _visualisationData.ActiveCluster);
                        }
                        else
                        {
                            wrapper.MarkCluster(_visualisationData.ActiveCluster, GlobalManager.CLUSTER_HIGHLIGHT_COLOR_2_ACTIVE);
                        }
                        wrapper.MarkCluster(!_visualisationData.ActiveCluster, "#00000000");
                    }
                    return wrapper;
                }
                return null;
            }
        }
        /// <summary>
        /// Gets a value indicating whether the merging animation is currently played.
        /// </summary>
        public bool IsMerging
        {
            get { return _visualisationData.IsMerging; }
        }

        /// <summary>
        /// Gets a value indicating whether the applying crossover animation is currently played.
        /// </summary>
        public bool IsApplyingCrossover
        {
            get { return _visualisationData.IsApplyingCrossover; }
        }

        private bool _isApplyingCrossoverRunning { get; set; }

        protected override bool isAnimating
        {
            get => IsMergingRunning || _isApplyingCrossoverRunning;
        }

        /// <summary>
        /// Creates an instance of the GOM iteration view model
        /// </summary>
        /// <param name="stateChanges">The state changes to be visualised</param>
        /// <param name="workingData">The data to visualise on</param>
        public GOMIterationViewModel(List<IGOMStateChange> stateChanges, IterationData workingData) : base()
        {
            _stateChanges = stateChanges;
            WorkingData = workingData.Clone();
            MaxStateChange = _stateChanges.Count - 1;

            _checkpoints = new Tuple<IterationData, GOMVisualisationData, IList<Message>>[MaxStateChange / CHECKPOINT_SPACING + 1];


            Thread calculationThread = new Thread(new ThreadStart(() =>
            {
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

            IList<Message> messages = new List<Message>();

            for (int i = 0; i <= MaxStateChange; i++)
            {
                var res = _stateChanges[i].Apply(workingIterationData, workingVisualisationData, true);

                messages.Add(res.Item2);
                if (i % CHECKPOINT_SPACING == 0)
                {
                    _checkpoints[i / CHECKPOINT_SPACING] = new Tuple<IterationData, GOMVisualisationData, IList<Message>>(
                        workingIterationData.Clone(),
                        (GOMVisualisationData)workingVisualisationData.Clone(),
                        new List<Message>(messages)
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
                    Thread t = new Thread(new ThreadStart(() =>
                    {
                        Thread.Sleep(GlobalManager.ANIMATION_TIME);
                        Dispatcher.UIThread.Invoke(() =>
                        {
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
                    Thread t = new Thread(new ThreadStart(() =>
                    {
                        Thread.Sleep(GlobalManager.ANIMATION_TIME);
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            _visualisationData.IsApplyingCrossover = false;
                            this.RaisePropertyChanged(nameof(IsApplyingCrossover));

                            _isApplyingCrossoverRunning = false;
                            RaiseButtonsChanged();
                        });
                    }));
                    t.Start();
                }
                else if (property.Equals(nameof(Solutions)))
                {
                    _stopAddSolution = _addSolutionRunning;
                    while (_addSolutionRunning)
                    {
                        Thread.Sleep(10);
                    }

                    Solutions.Clear();

                    Thread t = new Thread(new ThreadStart(() =>
                    {
                        LoadWrappersAsync(Solutions,
                            // Clone for use in another thread
                            new List<SolutionWrapper>(_visualisationData.Solutions),
                            ref _addSolutionRunning, ref _stopAddSolution);
                    }));
                    _addSolutionRunning = true;
                    t.Start();

                }
                else if (property.Equals(nameof(NextIteration)))
                {
                    _stopAddNextIteration = _addNextIterationRunning;
                    while (_addNextIterationRunning)
                    {
                        Thread.Sleep(10);
                    }

                    NextIteration.Clear();

                    Thread t = new Thread(new ThreadStart(() =>
                    {
                        LoadWrappersAsync(NextIteration,
                            // Clone for use in another thread
                            new List<SolutionWrapper>(_visualisationData.NextIteration),
                            ref _addNextIterationRunning, ref _stopAddNextIteration);
                    }));
                    t.Start();

                }
                else if (property.Equals(nameof(CurrentDonor)) || property.Equals(nameof(CurrentSolution)))
                {
                    this.RaisePropertyChanged(property + "Animated");
                }
                else if (property.Equals("NextIterationAdded"))
                {
                    NextIteration.Add(_visualisationData.NextIteration.Last());
                }

                this.RaisePropertyChanged(property);
            }
            RaiseButtonsChanged();
        }

        private void VisualizeCheckpoint(Tuple<IterationData, GOMVisualisationData, IList<Message>> checkPoint)
        {
            MessageBox.Clear();
            foreach (Message m in checkPoint.Item3)
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

        /// <summary>
        /// Changes the solution depiction.
        /// </summary>
        public override void ChangeSolutionDepiction()
        {
            foreach (var checkpoint in _checkpoints)
            {
                checkpoint.Item2.NextIteration.ToList().ForEach(d => d.IsBarCode = GlobalManager.Instance.IsBarCodeDepiction);
                checkpoint.Item2.Solutions.ToList().ForEach(d =>
                {
                    d.IsBarCode = GlobalManager.Instance.IsBarCodeDepiction;
                });
            }
            _visualisationData.NextIteration.ToList().ForEach(d => d.IsBarCode = GlobalManager.Instance.IsBarCodeDepiction);
            _visualisationData.Solutions.ToList().ForEach(d =>
            {
                d.IsBarCode = GlobalManager.Instance.IsBarCodeDepiction;
            });
            RaiseChanged(VISUALISATION_PROPERTIES);
        }
    }
}
