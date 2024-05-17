using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLEAV.Models;
using System.Diagnostics;
using Avalonia.Controls;

namespace LLEAV.ViewModels.Controls.PopulationDepictions
{
    public class PopulationGraph: PopulationContainerViewModelBase
    {
        public Axis[] XAxis { get; set; }
        public ISeries[] Series { get; set; }

        public PopulationGraph(IList<Population> populations, int iteration, bool[] flags) : base(populations.Count > 0 ? populations.Last(): null)
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
            XAxis = [
                new Axis()
                {
                    Labels = Enumerable.Range(
                        Math.Max(0, iteration + 1 - Math.Min(
                            PopulationGraphsViewModel.WINDOW_SIZE,
                            populations.Count)),
                        iteration + 1).Select(i => i.ToString()).ToList(),
                    ForceStepToMin = true,
                    MinStep = 1,
                },
                ];
        }

        public void SetVisible(int index, bool value)
        {
            Series[index].IsVisible = value;
        }
    }

    public class PopulationGraphsViewModel : PopulationDepictionViewModelBase
    {
        public const int WINDOW_SIZE = 10;

        private bool _averageScoreVisible = true;
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


        private RunData _runData;
        public PopulationGraphsViewModel(RunData runData)
        {
            _runData = runData;
        }

        public override void Update(IterationData iteration)
        {
            FindIndexOfSelected();
            Containers.Clear();

            for (int i = 0; i < iteration.Populations.Count; i++) 
            {
                List<Population> populations = new List<Population>();
                for (int j = 0; j < WINDOW_SIZE; j++)
                {
                    int index = iteration.Iteration - j;
                    if (index < 0 || _runData.Iterations[index].Populations.Count <= i) break;

                    populations.Insert(0, _runData.Iterations[index].Populations[i]);
                }
                Containers.Add(new PopulationGraph(populations, iteration.Iteration, new bool[] {
                    AverageScoreVisible,
                    MedianScoreVisible,
                    MaximumScoreVisible,
                    MinimumScoreVisible,
                }));
            }

            SelectPopulation(SelectedPopulation);
        }
    }
}
