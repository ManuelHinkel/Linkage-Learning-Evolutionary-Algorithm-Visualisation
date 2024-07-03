using Avalonia.Threading;
using LLEAV.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace LLEAV.ViewModels.Controls.IterationDepictions
{
    /// <summary>
    /// Base view model for managing iteration depiction in an algorithm visualization.
    /// </summary>
    public abstract class IterationDepictionViewModelBase : ViewModelBase
    {
        /// <summary>
        /// Constant defining the spacing between checkpoints.
        /// </summary>
        public const int CHECKPOINT_SPACING = 50;

        /// <summary>
        /// Gets a value indicating whether the right navigation button is enabled.
        /// </summary>
        public bool RightButtonEnabled
        {
            get => !isAnimating && !Running && CurrentStateChange < MaxStateChange;
        }

        /// <summary>
        /// Gets a value indicating whether the left navigation button is enabled.
        /// </summary>
        public bool LeftButtonEnabled
        {
            get => !isAnimating && !Running && CurrentStateChange > 0;
        }

        /// <summary>
        /// Gets a value indicating whether the forward navigation button is enabled.
        /// </summary>
        public bool ForwardButtonEnabled
        {
            get => !isAnimating && !Running
                && CurrentStateChange == MaxStateChange && GlobalManager.Instance.CanStepForward();
        }

        /// <summary>
        /// Gets a value indicating whether the backward navigation button is enabled.
        /// </summary>
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


        /// <summary>
        /// Gets the animation mode from the global manager instance.
        /// </summary>
        public AnimationModus AnimationModus
        {
            get => GlobalManager.Instance.AnimationModus;
        }

        /// <summary>
        /// Gets or sets a value indicating whether merging animation is running.
        /// </summary>
        [Reactive]
        public bool IsMergingRunning { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the animation is automatically playing.
        /// </summary>
        [Reactive]
        public bool Running { get; set; }


        protected int _currentStateChange;
        /// <summary>
        /// Gets or sets the current state change index.
        /// </summary>
        public int CurrentStateChange
        {
            get { return _currentStateChange; }
            set
            {
                GoToStateChange(value);
                this.RaisePropertyChanged(nameof(CurrentStateChange));
            }

        }

        /// <summary>
        /// Gets or sets the maximum state change index.
        /// </summary>
        public int MaxStateChange { get; set; }

        /// <summary>
        /// Gets or sets the message box instance for displaying messages.
        /// </summary>
        public MessageBox MessageBox { get; set; } = new MessageBox();

        /// <summary>
        /// Gets or sets the tick spacing for visualization.
        /// </summary>
        [Reactive]
        public int TickSpacing { get; private set; } = 1;

        protected IterationData WorkingData { get; set; }
        protected IterationData BaseData { get; set; }

        private bool _stopThread;

        protected IterationDepictionViewModelBase()
        {
            Thread playThread = new Thread(new ThreadStart(() =>
            {
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

        /// <summary>
        /// Stops the play thread forcefully.
        /// </summary>
        public void Stop()
        {
            _stopThread = true;
        }

        /// <summary>
        /// Toggles the running state of the iteration.
        /// </summary>
        public void Play()
        {
            Running = !Running;
            RaiseButtonsChanged();
        }

        /// <summary>
        /// Steps the state change forward.
        /// </summary>
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

        /// <summary>
        /// Steps the  state change backward.
        /// </summary>
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

        /// <summary>
        /// Advances the global step forward in the iteration.
        /// </summary>
        public void GlobalStepForward()
        {
            GlobalManager.Instance.StepForward();
            RaiseButtonsChanged();
        }

        /// <summary>
        /// Steps backward globally in the iteration.
        /// </summary>
        public void GlobalStepBackward()
        {
            GlobalManager.Instance.StepBackward();
            RaiseButtonsChanged();
        }

        protected void CalculateTickSpacing()
        {
            if (MaxStateChange > 2000)
            {
                TickSpacing = 200;
            }
            else if (MaxStateChange > 1000)
            {
                TickSpacing = 100;
            }
            else if (MaxStateChange > 500)
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

        protected void LoadWrappersAsync<T>(ObservableCollection<T> output, IList<T> input, ref bool running, ref bool stoppingCondition)
        {
            running = true;
            foreach (T w in input)
            {
                if (stoppingCondition)
                {
                    stoppingCondition = false;
                    running = false;
                    return;
                }
                // Call on UI for correct event handling
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    output.Add(w);
                });
                Thread.Sleep(50);
            }
            stoppingCondition = false;
            running = false;
        }

        /// <summary>
        /// Abstract method to change solution depiction, implemented by derived classes.
        /// </summary>
        public abstract void ChangeSolutionDepiction();
    }
}
