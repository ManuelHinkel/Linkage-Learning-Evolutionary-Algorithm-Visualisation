using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LLEAV.Models;
using ReactiveUI;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LLEAV.ViewModels.Controls.PopulationDepictions
{
    /// <summary>
    /// Represents the graph depiction of a population
    /// </summary>
    public class PopulationGraph : PopulationContainerViewModelBase
    {
        /// <summary>
        /// Gets or sets the X-axis configuration for the graph.
        /// </summary>
        public Axis[] XAxis { get; set; }

        /// <summary>
        /// Gets or sets the lines representing data to display on the graph.
        /// </summary>
        public ISeries[] Series { get; set; }

        /// <summary>
        /// Constructs a new instance of PopulationGraph.
        /// </summary>
        /// <param name="populations">List of populations to visualize.</param>
        /// <param name="iteration">Current iteration index.</param>
        /// <param name="flags">Flags indicating which series to display.</param>
        /// <param name="windowSize">Size of the sliding window of iterations to display.</param>
        public PopulationGraph(IList<Population> populations, int iteration, bool[] flags, int windowSize) : base(populations.Count > 0 ? populations.Last() : null)
        {
            Series = new ISeries[]
            {
                new LineSeries<double>
                {

                    Values = populations.Select(p => p.AverageFitness),
                    Fill = null,
                    LineSmoothness=0,
                    Name="Average",
                    Stroke = new SolidColorPaint(SKColors.Blue),
                    GeometryFill = new SolidColorPaint(SKColors.Blue),
                    GeometryStroke= new SolidColorPaint(SKColors.White),
                    IsVisible = flags[0],
                },

                new LineSeries<double>
                {
                    Values = populations.Select(p => p.MedianFitness),
                    Fill = null,
                    LineSmoothness=0,
                    Name="Median",
                    Stroke = new SolidColorPaint(SKColors.Green),
                    GeometryFill = new SolidColorPaint(SKColors.Green),
                    GeometryStroke= new SolidColorPaint(SKColors.White),
                    IsVisible = flags[1],
                },

                new LineSeries<double>
                {
                    Values = populations.Select(p => p.MaximumFitness),
                    Fill = null,
                    LineSmoothness=0,
                    Name="Maximum",
                    Stroke = new SolidColorPaint(SKColors.Yellow),
                    GeometryFill = new SolidColorPaint(SKColors.Yellow),
                    GeometryStroke= new SolidColorPaint(SKColors.White),
                    IsVisible = flags[2],
                },

                new LineSeries<double>
                {
                    Values = populations.Select(p => p.MinimumFitness),
                    Fill = null,
                    LineSmoothness=0,
                    Name="Minimum",
                    Stroke = new SolidColorPaint(SKColors.Purple),
                    GeometryFill = new SolidColorPaint(SKColors.Purple),
                    GeometryStroke= new SolidColorPaint(SKColors.White),
                    IsVisible = flags[3],
                },
            };

            int start = Math.Max(0, iteration + 1 - Math.Min(windowSize, populations.Count));

            XAxis = [
                new Axis()
                {
                    Labels = Enumerable.Range(
                        Math.Max(0, start),
                        Math.Min(windowSize, iteration + 1 - start)).Select(i => i.ToString()).ToList(),
                    ForceStepToMin = true,
                    MinStep = 1,
                },
            ];
        }

        /// <summary>
        /// Sets the visibility of a line on the graph.
        /// </summary>
        /// <param name="index">Index of the line to modify.</param>
        /// <param name="value">Visibility flag (true for visible, false for hidden).</param>
        public void SetVisible(int index, bool value)
        {
            Series[index].IsVisible = value;
        }
    }

    /// <summary>
    /// ViewModel for managing multiple population graphs based on provided run data.
    /// </summary>
    public class PopulationGraphsViewModel : PopulationDepictionViewModelBase
    {
        private int _windowSize = 10;
        /// <summary>
        /// Gets or sets the window size of iterations to display on each graph.
        /// </summary>
        public int WindowSize
        {
            get => _windowSize;

            set
            {
                this.RaiseAndSetIfChanged(ref _windowSize, value);
                if (_shownIteration != null)
                {
                    Update(_shownIteration);
                }
            }
        }

        private bool _averageScoreVisible = true;
        /// <summary>
        /// Gets or sets the visibility of the average score on all population graphs.
        /// </summary>
        public bool AverageScoreVisible
        {
            get => _averageScoreVisible;
            set
            {
                foreach (PopulationGraph g in Containers)
                {
                    g.SetVisible(0, value);
                }
                _averageScoreVisible = value;
            }
        }
        private bool _medianScoreVisible = true;
        /// <summary>
        /// Gets or sets the visibility of the median score on all population graphs.
        /// </summary>
        public bool MedianScoreVisible
        {
            get => _medianScoreVisible;
            set
            {
                foreach (PopulationGraph g in Containers)
                {
                    g.SetVisible(1, value);
                }
                _medianScoreVisible = value;
            }
        }
        private bool _maximumScoreVisible = true;
        /// <summary>
        /// Gets or sets the visibility of the maximum score on all population graphs.
        /// </summary>
        public bool MaximumScoreVisible
        {
            get => _maximumScoreVisible;
            set
            {
                foreach (PopulationGraph g in Containers)
                {
                    g.SetVisible(2, value);
                }
                _maximumScoreVisible = value;
            }
        }

        private bool _minimumScoreVisible = false;
        /// <summary>
        /// Gets or sets the visibility of the minimum score on all population graphs.
        /// </summary>
        public bool MinimumScoreVisible
        {
            get => _minimumScoreVisible;
            set
            {
                foreach (PopulationGraph g in Containers)
                {
                    g.SetVisible(3, value);
                }
                _minimumScoreVisible = value;
            }
        }


        private IterationData? _shownIteration;

        private RunData _runData;

        /// <summary>
        /// Constructs an instance of PopulationGraphsViewModel with the given run data.
        /// </summary>
        /// <param name="runData">Run data containing populations to visualize.</param>
        public PopulationGraphsViewModel(RunData runData)
        {
            _runData = runData;
        }

        /// <summary>
        /// Updates the ViewModel with data from a new iteration.
        /// </summary>
        /// <param name="iteration">Iteration data to visualize.</param>
        public override void Update(IterationData iteration)
        {
            _shownIteration = iteration;
            Containers.Clear();

            for (int i = 0; i < iteration.Populations.Count; i++)
            {
                List<Population> populations = new List<Population>();

                if (iteration.Iteration == -1)
                {
                    populations.Add(iteration.Populations[0]);
                }
                else
                {
                    for (int j = 0; j < WindowSize; j++)
                    {
                        int index = iteration.Iteration - j;
                        if (index < 0 || _runData.Iterations[index].Populations.Count <= i) break;

                        populations.Insert(0, _runData.Iterations[index].Populations[i]);
                    }
                }
                Containers.Add(new PopulationGraph(populations, iteration.Iteration, new bool[] {
                    AverageScoreVisible,
                    MedianScoreVisible,
                    MaximumScoreVisible,
                    MinimumScoreVisible,
                }, WindowSize));
            }

            SelectPopulation(SelectedPopulation);
        }
    }
}
