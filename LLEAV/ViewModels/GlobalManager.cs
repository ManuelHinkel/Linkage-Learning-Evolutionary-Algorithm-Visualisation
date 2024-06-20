using Avalonia.Threading;
using LLEAV.Models;
using LLEAV.Models.Algorithms;
using LLEAV.ViewModels.Windows;
using LLEAV.Views.Windows;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Threading;

namespace LLEAV.ViewModels
{
    /// <summary>
    /// Enumeration for the different animation modi.
    /// </summary>
    public enum AnimationModus
    {
        FULL,
        FOS,
        NONE
    }

    /// <summary>
    /// Class which manages the application.
    /// </summary>
    public class GlobalManager
    {
        public const string TEXT_COLOR = "#25020a";
        public const string DEFAULT_WHITE = "#feedf2";
        public const string PRIMARY_COLOR = "#ff8566";
        public const string SECONDARY_COLOR = "#f9b178";
        public const string TERTIARY_COLOR = "#f7bd45";

        public const string GRAY = "#444444";

        /// <summary>
        /// Color to highlight the active bits used in a merge from the parent.
        /// </summary>
        public const string CLUSTER_HIGHLIGHT_COLOR_1_ACTIVE = "#84fc03";

        /// <summary>
        /// Color to highlight the active bits used in a merge from the donor.
        /// </summary>
        public const string CLUSTER_HIGHLIGHT_COLOR_2_ACTIVE = "#03f8fc";

        /// <summary>
        /// Color to highlight the inactive bits used in a merge from the parent.
        /// </summary>
        public const string CLUSTER_HIGHLIGHT_COLOR_1_INACTIVE = "#1a4f20";

        /// <summary>
        /// Color to highlight the inactive bits used in a merge from the donor.
        /// </summary>
        public const string CLUSTER_HIGHLIGHT_COLOR_2_INACTIVE = "#1600a3";

        /// <summary>
        /// Colors used for the messages.
        /// </summary>
        public static readonly string[] MESSAGE_COLORS = [
            SECONDARY_COLOR,
            CLUSTER_HIGHLIGHT_COLOR_1_ACTIVE,
            CLUSTER_HIGHLIGHT_COLOR_2_ACTIVE,
        ];

        /// <summary>
        /// The time of an animation.
        /// </summary>
        public const int ANIMATION_TIME = 500;

        private static GlobalManager? _instance;
        /// <summary>
        /// Gets the instance of the global manager.
        /// </summary>
        public static GlobalManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GlobalManager();
                }
                return _instance;
            }
        }

        private AnimationModus _animationModus;
        /// <summary>
        /// Gets or sets the animation modus.
        /// </summary>
        public AnimationModus AnimationModus
        {
            get => _animationModus;
            set
            {
                _animationModus = value;
                _mainWindowViewModel.RaisePropertyChanged(nameof(AnimationModus));
                if (_populationWindowViewModel != null)
                {
                    _populationWindowViewModel.RaisePropertyChanged(nameof(AnimationModus));
                }
                if (_iterationDetailWindowViewModel != null && _iterationDetailWindowViewModel.ContentViewModel != null)
                {
                    _iterationDetailWindowViewModel.ContentViewModel.RaisePropertyChanged(nameof(AnimationModus));
                }
            }
        }

        private bool _isBarCodeDepiction;
        /// <summary>
        /// Gets or sets, if the solutions should be shown in bar code depiction.
        /// </summary>
        public bool IsBarCodeDepiction
        {
            get => _isBarCodeDepiction;
            set
            {
                _isBarCodeDepiction = value;
                if (_populationWindowViewModel != null)
                {
                    _populationWindowViewModel.ChangeSolutionDepiction();
                }
                if (_iterationDetailWindowViewModel != null && _iterationDetailWindowViewModel.ContentViewModel != null)
                {
                    _iterationDetailWindowViewModel.ChangeSolutionDepiction();
                }
            }
        }
        /// <summary>
        /// Gets or sets the current iteration.
        /// </summary>
        public int CurrentIteration { get; set; }

        /// <summary>
        /// Gets, if iteration details are being animated.
        /// </summary>
        public bool IsAnimatingDetails { get; private set; }

        /// <summary>
        /// Gets, if the FOS structure is being animated.
        /// </summary>
        public bool IsAnimatingFOS { get; set; }

        private MainWindowViewModel? _mainWindowViewModel;
        private PopulationWindowViewModel? _populationWindowViewModel;
        private IterationDetailWindowViewModel? _iterationDetailWindowViewModel;

        /// <summary>
        /// The main window of the application.
        /// </summary>
        public MainWindow MainWindow;

        // Close when main window closes
        private PopulationWindow? _populationWindow;
        // Close when main window closes
        private IterationDetailWindow? _iterationDetailWindow;

        private bool _newAlgorithmWindowOpen;
        private RunData? _runData;

        private bool _threadRunning;

        /// <summary>
        /// Creates a new instance of the global manager.
        /// </summary>
        public GlobalManager()
        {
            MainWindow = new MainWindow();
            _mainWindowViewModel = new MainWindowViewModel();
            MainWindow.DataContext = _mainWindowViewModel;
            MainWindow.Closing += (s, e) =>
            {
                _mainWindowViewModel.Stop();
                if (_populationWindow != null) _populationWindow.Close();
                if (_iterationDetailWindow != null) _iterationDetailWindow.Close();
            };
        }

        /// <summary>
        /// Open the population window.
        /// </summary>
        public void OpenPopulationWindow()
        {
            if (_populationWindow == null)
            {
                _populationWindow = new PopulationWindow();
                _populationWindow.Show();
                _populationWindowViewModel = new PopulationWindowViewModel();
                _populationWindow.DataContext = _populationWindowViewModel;
                _populationWindow.Closing += (s, e) =>
                {
                    _populationWindow = null;
                    // Clear selected highlighting
                    _mainWindowViewModel!.SelectPopulation(-1);
                };
            }
        }

        /// <summary>
        /// Opens a specific population in the population window.
        /// </summary>
        /// <param name="population">The population to show.</param>
        public void OpenPopulation(Population population)
        {
            OpenPopulationWindow();

            UpdatePopulationWindowIfOpen(population);
        }


        /// <summary>
        /// Selects a population of the current iteration.
        /// </summary>
        /// <param name="data">The iteration data containing the populations to select from.</param>
        /// <param name="index">The index of the population to select.</param>
        public void SelectPopulation(IterationData data, int index)
        {
            UpdateMainWindow(data);
            _mainWindowViewModel.SelectPopulation(index);
        }

        /// <summary>
        /// Selects a cluster and updates UI accordingly.
        /// </summary>
        /// <param name="index">Index of the population to select.</param>
        /// <param name="cluster">Cluster to mark.</param>
        public void SelectCluster(int index, Cluster cluster)
        {
            _mainWindowViewModel.SelectPopulation(index);
            _populationWindowViewModel.MarkCluster(cluster);
        }

        /// <summary>
        /// Updates the population window if it's open, animating the changes over a period of time.
        /// </summary>
        /// <param name="population">The population to set in the population window.</param>
        public void UpdatePopulationWindowIfOpen(Population population)
        {
            if (_populationWindow != null)
            {
                IsAnimatingFOS = true;

                _populationWindowViewModel!.SetPopulation(population);

                if (!_threadRunning)
                {
                    _threadRunning = true;
                    Thread t = new Thread(new ThreadStart(() =>
                    {
                        Thread.Sleep(ANIMATION_TIME);
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            IsAnimatingFOS = false;
                        });
                        _threadRunning = false;
                    }));
                    t.Start();
                }
            }
            else
            {
                IsAnimatingFOS = false;
            }
        }

        /// <summary>
        /// Opens the iteration detail window.
        /// </summary>
        public void OpenIterationDetailWindow()
        {
            OpenPopulationWindow();
            if (_iterationDetailWindow == null)
            {
                _iterationDetailWindow = new IterationDetailWindow();
                _iterationDetailWindow.Show();
                _iterationDetailWindowViewModel = new IterationDetailWindowViewModel();
                _iterationDetailWindow.DataContext = _iterationDetailWindowViewModel;
                _iterationDetailWindow.Closing += (s, e) =>
                {
                    _iterationDetailWindowViewModel.Stop();
                    _iterationDetailWindow = null;
                    NotifyFinishedIteration();
                };
            }
        }
        /// <summary>
        /// Starts the iteration visualization, setting detailed data in the iteration detail window.
        /// </summary>
        /// <param name="stateChanges">List of state changes.</param>
        public void StartIterationVisualization(IList<IStateChange> stateChanges)
        {

            IsAnimatingDetails = true;
            OpenIterationDetailWindow();

            IterationData workingCopy = CurrentIteration > 0 ? _runData!.Iterations[CurrentIteration - 1].Clone() : InitialIteration();

            UpdateMainWindow(workingCopy);

            _iterationDetailWindowViewModel!.SetDetailedData(stateChanges,
                workingCopy,
                _runData);
        }

        /// <summary>
        /// Notifies that the current iteration has finished, updating the main window.
        /// </summary>
        public void NotifyFinishedIteration()
        {
            UpdateMainWindow(_runData.Iterations[CurrentIteration]);
            IsAnimatingDetails = false;
        }

        /// <summary>
        /// Opens a new algorithm configuration window.
        /// </summary>
        public void OpenNewAlgorithmWindow()
        {
            if (!_newAlgorithmWindowOpen)
            {
                var w = new NewAlgorithmWindow();
                w.Show(MainWindow);
                w.DataContext = new NewAlgorithmWindowViewModel(w);
                _newAlgorithmWindowOpen = true;
                w.Closing += (s, e) => { _newAlgorithmWindowOpen = false; };
            }
        }

        /// <summary>
        /// Sets new run data and updates the main window view model.
        /// </summary>
        /// <param name="runData">The new run data to set.</param>
        public void SetNewRunData(RunData runData)
        {
            _runData = runData;
            _mainWindowViewModel!.SetNewRunData(_runData);
        }

        /// <summary>
        /// Updates the main window view model with iteration data to show.
        /// </summary>
        /// <param name="iterationData">The iteration data to update.</param>
        public void UpdateMainWindow(IterationData iterationData)
        {
            _mainWindowViewModel!.UpdatePopulations(iterationData);
        }

        /// <summary>
        /// Steps forward to the next iteration.
        /// </summary>
        public void StepForward()
        {
            _mainWindowViewModel.StepForward();
        }

        /// <summary>
        /// Steps backward to the last iteration.
        /// </summary>
        public void StepBackward()
        {
            _mainWindowViewModel.StepBackward();
        }

        /// <summary>
        /// Checks if it's possible to step backward in the iteration data.
        /// </summary>
        /// <returns>True if stepping backward is possible, otherwise false.</returns>
        public bool CanStepBackward()
        {
            return CurrentIteration > 0;
        }

        /// <summary>
        /// Checks if it's possible to step forward in the iteration data.
        /// </summary>
        /// <returns>True if stepping forward is possible, otherwise false.</returns>
        public bool CanStepForward()
        {
            return !_runData.Iterations[CurrentIteration].LastIteration;
        }

        /// <summary>
        /// Generates the initial iteration data using the current run data.
        /// </summary>
        /// <returns>The initial iteration data.</returns>
        public IterationData InitialIteration()
        {
            Random rng = new Random(_runData.RNGSeed);

            return new IterationData(_runData.Algorithm.InitialPopulation(_runData, rng), rng.Next());
        }
    }
}
