using Avalonia.Threading;
using LiveChartsCore.Kernel;
using LLEAV.Models;
using LLEAV.Models.Algorithms;
using LLEAV.Models.Algorithms.MIP.StateChange;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        ];

        protected override IList<string> animationProperties { get; } = [
            "IsMerging",
            "IsApplyingCrossover",
        ];

        private IList<IMIPStateChange> _stateChanges;
        private MIPVisualisationData _visualisationData = new MIPVisualisationData();

        private Tuple<IterationData, MIPVisualisationData, IList<Message>>[] _checkpoints;

        private bool _stopAddSolution;
        private bool _addSolutionRunning;
        public ObservableCollection<Tuple<SolutionWrapper,SolutionWrapper>> Solutions { get; } = new ObservableCollection<Tuple<SolutionWrapper, SolutionWrapper>>();

        private bool _stopAddDonor;
        private bool _addDonorRunning;
        public ObservableCollection<SolutionWrapper> Donors { get; } = [];

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
                            wrapper.MarkCluster(GlobalManager.CLUSTER_HIGHLIGHT_COLOR_2_ACTIVE, GlobalManager.CLUSTER_HIGHLIGHT_COLOR_2_INACTIVE,
                                _visualisationData.ActiveCluster);
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
                        }else
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
                            wrapper.MarkCluster(GlobalManager.CLUSTER_HIGHLIGHT_COLOR_2_ACTIVE, GlobalManager.CLUSTER_HIGHLIGHT_COLOR_2_INACTIVE,
                                _visualisationData.ActiveCluster);
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

        public MIPIterationViewModel(IList<IMIPStateChange> stateChanges, IterationData workingData): base()
        {
            _stateChanges = stateChanges;
            WorkingData = workingData.Clone();
            MaxStateChange = _stateChanges.Count - 1;

            _checkpoints = new Tuple<IterationData, MIPVisualisationData, IList<Message>>[MaxStateChange / CHECKPOINT_SPACING + 1];


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
            MIPVisualisationData workingVisualisationData = new MIPVisualisationData();

            IList<Message> messages = new List<Message>();

            for (int i = 0; i <= MaxStateChange; i++)
            {
                var res = _stateChanges[i].Apply(workingIterationData, workingVisualisationData, true);

                messages.Add(res.Item2);
                if (i % CHECKPOINT_SPACING == 0)
                {
                    _checkpoints[i / CHECKPOINT_SPACING] = new Tuple<IterationData, MIPVisualisationData, IList<Message>>(
                        workingIterationData.Clone(),
                        (MIPVisualisationData)workingVisualisationData.Clone(),
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
                else if (property.Equals(nameof(Solutions)))
                {
                    _stopAddSolution = _addSolutionRunning;
                    while (_addSolutionRunning)
                    {
                        Debug.WriteLine("wait2");
                        Thread.Sleep(10);
                    }

                    Solutions.Clear();

                    Thread t = new Thread(new ThreadStart(() => {
                        LoadWrappersAsync(Solutions,
                            // Clone for use in another thread
                            new List<Tuple<SolutionWrapper, SolutionWrapper>>(_visualisationData.Solutions),
                            ref _addSolutionRunning, ref _stopAddSolution);
                    }));
                    _addSolutionRunning = true;
                    t.Start();

                }
                else if (property.Equals(nameof(Donors)))
                {
                    _stopAddDonor = _addDonorRunning;
                    while (_addDonorRunning)
                    {
                        Debug.WriteLine("wait1");
                        Thread.Sleep(10);
                    }

                    Donors.Clear();

                    Thread t = new Thread(new ThreadStart(() => {
                        LoadWrappersAsync(Donors,
                            // Clone for use in another thread
                            new List<SolutionWrapper>(_visualisationData.Donors),
                            ref _addDonorRunning, ref _stopAddDonor);
                    }));
                    t.Start();

                }
                else if (property.Equals(nameof(CurrentDonor)) || property.Equals(nameof(CurrentSolution)))
                {
                    this.RaisePropertyChanged(property + "Animated");
                }



                this.RaisePropertyChanged(property);
            }
            RaiseButtonsChanged();
        }

        private void VisualizeCheckpoint(Tuple<IterationData, MIPVisualisationData, IList<Message>> checkPoint)
        {
            MessageBox.Clear();
            foreach (Message m in checkPoint.Item3)
            {
                MessageBox.Insert(0, m);
            }

            if (checkPoint.Item2.ViewedPopulation != null)
            {
                GlobalManager.Instance.SelectPopulation(checkPoint.Item1, checkPoint.Item2.ViewedPopulation.PyramidIndex);
                if (checkPoint.Item2.ActiveCluster != null)
                {
                    GlobalManager.Instance.SelectCluster(checkPoint.Item2.ViewedPopulation.PyramidIndex, checkPoint.Item2.ActiveCluster);
                }
            }

            WorkingData = checkPoint.Item1.Clone();
            _visualisationData = (MIPVisualisationData)checkPoint.Item2.Clone();

            RaiseChanged(VISUALISATION_PROPERTIES);
        }

        protected override void GoToStateChange(int index)
        {
            if (_currentStateChange == index) return;

            int checkpointIndex = _currentStateChange / CHECKPOINT_SPACING;
            int targetCheckpointIndex = index / CHECKPOINT_SPACING;

            if ((index != _currentStateChange + 1) &&  (checkpointIndex != targetCheckpointIndex || index < _currentStateChange))
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
        public override void ChangeSolutionDepiction()
        {
            foreach (var checkpoint in _checkpoints)
            {
                checkpoint.Item2.Donors.ToList().ForEach(d => d.IsBarCode = GlobalManager.Instance.IsBarCodeDepiction);
                checkpoint.Item2.Solutions.ToList().ForEach(d =>
                {
                    d.Item1.IsBarCode = GlobalManager.Instance.IsBarCodeDepiction;
                    d.Item2.IsBarCode = GlobalManager.Instance.IsBarCodeDepiction;
                });
            }
            _visualisationData.Donors.ToList().ForEach(d => d.IsBarCode = GlobalManager.Instance.IsBarCodeDepiction);
            _visualisationData.Solutions.ToList().ForEach(d =>
            {
                d.Item1.IsBarCode = GlobalManager.Instance.IsBarCodeDepiction;
                d.Item2.IsBarCode = GlobalManager.Instance.IsBarCodeDepiction;
            });
            RaiseChanged(VISUALISATION_PROPERTIES);
        }
    }
}
