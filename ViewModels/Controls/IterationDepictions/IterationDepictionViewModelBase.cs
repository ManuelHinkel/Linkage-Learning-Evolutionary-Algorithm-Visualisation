using Avalonia.Threading;
using LLEAV.Models;
using LLEAV.Models.Algorithms;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;

namespace LLEAV.ViewModels.Controls.IterationDepictions
{
    public abstract class IterationDepictionViewModelBase : ViewModelBase
    {
        public const int CHECKPOINT_SPACING = 50;
        public const string CLUSTER_HIGHLIGHT_COLOR_1 = "#0000ff";
        public const string CLUSTER_HIGHLIGHT_COLOR_2 = "#00ff00";
        public bool RightButtonEnabled
        {
            get => !isAnimating && !Running  && CurrentStateChange < MaxStateChange;
        }

        public bool LeftButtonEnabled
        {
            get => !isAnimating && !Running  && CurrentStateChange > 0;
        }

        public bool ForwardButtonEnabled
        {
            get => !isAnimating && !Running 
                && CurrentStateChange == MaxStateChange && GlobalManager.Instance.CanStepForward();
        }

        public bool BackwardButtonEnabled
        {
            get => !isAnimating && !Running 
                && CurrentStateChange == 0 && GlobalManager.Instance.CanStepBackward();
        }

        protected abstract bool isAnimating
        {
            get;
        }

        protected abstract IList<string> animationProperties
        {
            get;
        }


        public AnimationModus AnimationModus
        {
            get => GlobalManager.Instance.AnimationModus;
        }

            [Reactive]
        public bool IsMergingRunning { get; set; }


        [Reactive]
        public bool Running { get; set; }


        protected int _currentStateChange;
        public int CurrentStateChange 
        {
            get { return _currentStateChange; }
            set
            {
                GoToStateChange(value);
                this.RaisePropertyChanged(nameof(CurrentStateChange));
            } 
        
        }

        public int MaxStateChange { get; set; }

        public MessageBox MessageBox { get; set; } = new MessageBox();

        [Reactive]
        public int TickSpacing { get; private set; } = 1;

        protected IterationData WorkingData { get; set; }
        protected IterationData BaseData { get; set; }

        private bool _stopThread;

        protected IterationDepictionViewModelBase()
        {
            Thread playThread = new Thread(new ThreadStart(() => {
                while (!_stopThread)
                {
                    if (Running && !isAnimating)
                    {
                        Dispatcher.UIThread.Invoke(StepForward);
                    }
                    Thread.Sleep(10);
                }
            }));
            playThread.Start();
        }

        public void Stop()
        {
            _stopThread = true;
        }

        public void Play()
        {            
            Running = !Running;
        }

        public void StepForward()
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

        public void StepBackward()
        {
            if (CurrentStateChange > 0)
            {
                CurrentStateChange--;
            }
            RaiseButtonsChanged();
        }

        protected abstract void RaiseChanged(IList<string> properties);

        protected void RaiseButtonsChanged()
        {
            this.RaisePropertyChanged(nameof(RightButtonEnabled));
            this.RaisePropertyChanged(nameof(LeftButtonEnabled));
            this.RaisePropertyChanged(nameof(ForwardButtonEnabled));
            this.RaisePropertyChanged(nameof(BackwardButtonEnabled));
        }

        protected abstract void GoToStateChange(int index);

        public void GlobalStepForward()
        {
            GlobalManager.Instance.StepForward();
            RaiseButtonsChanged();
        }

        public void GlobalStepBackward()
        {
            GlobalManager.Instance.StepBackward();
            RaiseButtonsChanged();
        }

        protected void CalculateTickSpacing()
        {
            if (MaxStateChange > 500)
            {
                TickSpacing = 50;
            }
            else if (MaxStateChange > 250)
            {
                TickSpacing = 20;
            }
            else if (MaxStateChange > 100)
            {
                TickSpacing = 10;
            }
            else if (MaxStateChange > 50)
            {
                TickSpacing = 5;
            }
        }

        protected void CombineProperties(IList<string> newProperties, ISet<string> oldProperties)
        {
            oldProperties.ExceptWith(animationProperties);
            oldProperties.UnionWith(newProperties);
        }
    }
}
