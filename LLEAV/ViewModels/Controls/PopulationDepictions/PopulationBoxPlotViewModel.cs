using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LLEAV.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LLEAV.ViewModels.Controls.PopulationDepictions
{
    /// <summary>
    /// Represents the box plot depiction of a population
    /// </summary>
    public class PopulationBoxPlot : PopulationContainerViewModelBase
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
        /// Constructs a new instance of PopulationBoxPlot.
        /// </summary>
        /// <param name="populations">List of populations to visualize.</param>
        /// <param name="iteration">Current iteration index.</param>
        /// <param name="windowSize">Size of the sliding window of iterations to display.</param>
        public PopulationBoxPlot(IList<Population> populations, int iteration, int windowSize) : base(populations.Count > 0 ? populations.Last() : null)
        {
            Series = new ISeries[]
            {
                new BoxSeries<BoxValue>
                {
                    Name = "FItness Values",
                    Values = populations.Select(p => CalculateBoxValuesForPopulation(p)),
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

        private BoxValue CalculateBoxValuesForPopulation(Population population)
        {
            if (population.Count() == 0) return null;
            List<Solution> orderedDesc = new List<Solution>(population.Solutions).OrderByDescending(s => s.Fitness).ToList();

            // Solutions are ordered in descending order, therefore first quartil is in the second half
            double firstQuartil = orderedDesc[(orderedDesc.Count * 3) / 4].Fitness;

            // Solutions are ordered in descending order, therefore third quartil is in the first half
            double thirdQuartil = orderedDesc[orderedDesc.Count / 4].Fitness;

            return new BoxValue(population.MaximumFitness, thirdQuartil, firstQuartil, population.MinimumFitness, population.MedianFitness);
        }
    }

    /// <summary>
    /// ViewModel for managing multiple population box plots based on provided run data.
    /// </summary>
    public class PopulationBoxPlotViewModel : PopulationDepictionViewModelBase
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
        private IterationData? _shownIteration;

        private RunData _runData;

        /// <summary>
        /// Constructs an instance of PopulationBoxPlotViewModel with the given run data.
        /// </summary>
        /// <param name="runData">Run data containing populations to visualize.</param>
        public PopulationBoxPlotViewModel(RunData runData)
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
                Containers.Add(new PopulationBoxPlot(populations, iteration.Iteration, WindowSize));
            }

            SelectPopulation(SelectedPopulation);
        }
    }
}
