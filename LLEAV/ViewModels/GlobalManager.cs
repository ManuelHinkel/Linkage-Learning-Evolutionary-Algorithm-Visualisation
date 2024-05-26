﻿using Avalonia.Controls;
using Avalonia.Threading;
using LLEAV.Models;
using LLEAV.Models.Algorithms;
using LLEAV.ViewModels.Windows;
using LLEAV.Views.Windows;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace LLEAV.ViewModels
{
    public enum PopulationDepiction
    {
        BLOCK,
        GRAPH,
        BARS
    }

    public enum AnimationModus
    {
        FULL,
        FOS,
        NONE
    }

    public class GlobalManager
    {
        public const string TEXT_COLOR = "#25020a";
        public const string DEFAULT_WHITE = "#feedf2";
        public const string PRIMARY_COLOR = "#f50f54";
        public const string SECONDARY_COLOR = "#f9b178";
        public const string TERTIARY_COLOR = "#f7bd45";

        public const string CLUSTER_HIGHLIGHT_COLOR_1_ACTIVE = "#84fc03";
        public const string CLUSTER_HIGHLIGHT_COLOR_2_ACTIVE = "#03f8fc";

        public const string CLUSTER_HIGHLIGHT_COLOR_1_INACTIVE = "#1a4f20";
        public const string CLUSTER_HIGHLIGHT_COLOR_2_INACTIVE = "#1600a3";


        public static readonly string[] MESSAGE_COLORS = [
            SECONDARY_COLOR,
            CLUSTER_HIGHLIGHT_COLOR_1_ACTIVE,
            CLUSTER_HIGHLIGHT_COLOR_2_ACTIVE,
        ];

        public const int ANIMATION_TIME = 500;
        private static GlobalManager? _instance;
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

        public PopulationDepiction PopulationDepiction { get; set; }

        public int CurrentIteration { get; set; }
        public bool IsAnimatingDetails { get; private set; }
        public bool IsAnimatingFOS { get; set; }

        private MainWindowViewModel? _mainWindowViewModel;
        private PopulationWindowViewModel? _populationWindowViewModel;
        private IterationDetailWindowViewModel? _iterationDetailWindowViewModel;

        // Used to set as parent in New Algorithm Window.
        public MainWindow MainWindow;
        // Close when main window closes
        private PopulationWindow? _populationWindow;
        // Close when main window closes
        private IterationDetailWindow? _iterationDetailWindow;

        private bool _newAlgorithmWindowOpen;
        private RunData? _runData;

        private bool _threadRunning;

        public GlobalManager()
        {
            MainWindow = new MainWindow();
            _mainWindowViewModel = new MainWindowViewModel();
            MainWindow.DataContext = _mainWindowViewModel;
            MainWindow.Closing += (s, e) => { 
                _mainWindowViewModel.Stop();
                if (_populationWindow != null) _populationWindow.Close();
                if (_iterationDetailWindow != null) _iterationDetailWindow.Close();
            };
        }

        public void OpenPopulationWindow()
        {
            if (_populationWindow == null)
            {
                _populationWindow = new PopulationWindow();
                _populationWindow.Show();
                _populationWindowViewModel = new PopulationWindowViewModel();
                _populationWindow.DataContext = _populationWindowViewModel;
                _populationWindow.Closing += (s, e) => { 
                    _populationWindow = null; 
                    // Clear selected highlighting
                    _mainWindowViewModel!.SelectPopulation(-1);
                    };
            }
        }

        public void OpenPopulation(Population population)
        {
            OpenPopulationWindow();

            UpdatePopulationWindowIfOpen(population);
        }



        public void SelectPopulation(IterationData data, int index)
        {
            UpdateMainWindow(data);
            _mainWindowViewModel.SelectPopulation(index);
        }

        public void SelectCluster(int index, Cluster cluster)
        {
            _mainWindowViewModel.SelectPopulation(index);
            _populationWindowViewModel.MarkCluster(cluster);
        }

        public void UpdatePopulationWindowIfOpen(Population population)
        {
            if (_populationWindow != null)
            {
                IsAnimatingFOS = true;

                _populationWindowViewModel!.SetPopulation(population);

                if (!_threadRunning)
                {
                    _threadRunning = true;
                    Thread t = new Thread(new ThreadStart(() => {
                        Thread.Sleep(ANIMATION_TIME);
                        Dispatcher.UIThread.Invoke(() => {
                            IsAnimatingFOS = false;
                        });
                        _threadRunning = false;
                    }));
                    t.Start();
                }
            } else
            {
                IsAnimatingFOS = false;
            }
        }

        public void OpenIterationDetailWindow()
        {
            OpenPopulationWindow();
            if (_iterationDetailWindow == null)
            {
                _iterationDetailWindow = new IterationDetailWindow();
                _iterationDetailWindow.Show();
                _iterationDetailWindowViewModel = new IterationDetailWindowViewModel();
                _iterationDetailWindow.DataContext = _iterationDetailWindowViewModel;
                _iterationDetailWindow.Closing += (s, e) => { 
                    _iterationDetailWindowViewModel.Stop();
                    _iterationDetailWindow = null;
                    NotifyFinishedIteration();
                };
            }
        }

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

        public void NotifyFinishedIteration()
        {
            UpdateMainWindow(_runData.Iterations[CurrentIteration]);
            IsAnimatingDetails = false;
        }

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

        public void SetNewRunData(RunData runData)
        {
            _runData = runData;
            _mainWindowViewModel!.SetNewRunData(_runData);
        }

        public void UpdateMainWindow(IterationData iterationData)
        {
            _mainWindowViewModel!.UpdatePopulations(iterationData);
        }

        public void StepForward()
        {
            _mainWindowViewModel.StepForward();
        }

        public void StepBackward()
        {
            _mainWindowViewModel.StepBackward();
        }

        public bool CanStepBackward()
        {
            return CurrentIteration > 0;
        }

        public bool CanStepForward()
        {
            return !_runData.Iterations[CurrentIteration].LastIteration;
        }

        public IterationData InitialIteration()
        {
            Random rng = new Random(_runData.RNGSeed);

            return new IterationData(_runData.Algorithm.InitialPopulation(_runData, rng), /*rng.Next()*/ 0);
        }
    }
}