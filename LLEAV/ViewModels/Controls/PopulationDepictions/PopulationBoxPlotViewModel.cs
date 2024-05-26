using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LLEAV.Models;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore.Defaults;
using ReactiveUI;
using System.Diagnostics;

namespace LLEAV.ViewModels.Controls.PopulationDepictions
{
    public class PopulationBoxPlot : PopulationContainerViewModelBase
    {
        public Axis[] XAxis { get; set; }
        public ISeries[] Series { get; set; }

        public PopulationBoxPlot(IList<Population> populations, int iteration, int windowSize) : base(populations.Count > 0 ? populations.Last(): null)
        {
            Series = new ISeries[]
            {
                new BoxSeries<BoxValue>
                {
                    Name = "FItness Values",
                    Values = populations.Select(p => CalculateBoxValuesForPopulation(p)),
                },
            };
            XAxis = [
               new Axis()
               {
                   Labels = Enumerable.Range(
                       Math.Max(0, iteration + 1 - Math.Min(
                           windowSize,
                           populations.Count)),
                       iteration + 1).Select(i => i.ToString()).ToList(),
                   ForceStepToMin = true,
                   MinStep = 1,
               },
            ];
        }

        private BoxValue CalculateBoxValuesForPopulation(Population population)
        {
            if (population.Count() == 0) return null;
            List<Solution> orderedDesc = new List<Solution>(population.Solutions).OrderByDescending(s => s.Fitness).ToList();

            // Solutions are ordered in descending order, therefore first quartil is in the second half
            double firstQuartil = orderedDesc[(orderedDesc.Count * 3) / 4].Fitness;

            // Solutions are ordered in descending order, therefore third quartil is in the first half
            double thirdQuartil = orderedDesc[orderedDesc.Count / 4].Fitness;

            return new BoxValue(population.MaximumFitness, thirdQuartil , firstQuartil, population.MinimumFitness, population.MedianFitness);
        }
    }

    public class PopulationBoxPlotViewModel : PopulationDepictionViewModelBase
    {
        private int _windowSize = 10;
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
        private IterationData? _shownIteration;

        private RunData _runData;
        public PopulationBoxPlotViewModel(RunData runData)
        {
            _runData = runData;
        }
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
                Containers.Add(new PopulationBoxPlot(populations, iteration.Iteration, WindowSize));
            }

            SelectPopulation(SelectedPopulation);
        }
    }
}
