using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLEAV.Models;
using System.Diagnostics;

namespace LLEAV.ViewModels.Controls.PopulationDepictions
{
    /// <summary>
    /// Represents the bar depiction of a population
    /// </summary>
    public class PopulationBar: PopulationContainerViewModelBase
    {
        private const double EPSILON = 0.0001;

        /// <summary>
        /// Gets or sets the X-axis configuration for the graph.
        /// </summary>
        public Axis[] XAxis { get; set; }

        /// <summary>
        /// Gets or sets the Y-axis configuration for the graph.
        /// </summary>
        public Axis[] YAxis { get; set; }

        /// <summary>
        /// Gets or sets the lines representing data to display on the graph.
        /// </summary>
        public ISeries[] Series { get; set; }

        /// <summary>
        /// Constructs a new instance of PopulationBar.
        /// </summary>
        /// <param name="population">The population assigned to the container.</param>
        public PopulationBar(Population population) : base(population)
        {
            var bars = CalculateBars(population);
            Series = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = bars,
                    Fill =  new SolidColorPaint(SKColors.CornflowerBlue),
                    Stroke = new SolidColorPaint(SKColors.CornflowerBlue),
                },
            };

            XAxis = [
                new Axis()
                {
                    Labels = CalculateLabels(population),
                    ForceStepToMin = true,
                    MinStep = 1,
                },
            ];


            YAxis = [
                new Axis()
                {
                    MinLimit = 0,
                }
            ];
        }
        private IList<string> CalculateLabels(Population population)
        {
            if (double.IsNaN(population.MinimumFitness) || double.IsNaN(population.MaximumFitness)) return [];

            int min = (int)Math.Floor(population.MinimumFitness);
            int max = (int)Math.Ceiling(population.MaximumFitness);
            if (min == max) max++;
            double bucketRange = (max - min) /(double) PopulationBarsViewModel.BAR_COUNT;

            IList<string> labels = new List<string>();

            double l = min + bucketRange;

            for(int i = 0; i  < PopulationBarsViewModel.BAR_COUNT; i++)
            {
                labels.Add(l.ToString("0.##"));
                l += bucketRange;
            }
            return labels;
        }

        private IEnumerable<int> CalculateBars(Population population)
        {
            int min = (int)Math.Floor(population.MinimumFitness);
            int max = (int)Math.Ceiling(population.MaximumFitness);
            double bucketRange = (max - min) / (double)PopulationBarsViewModel.BAR_COUNT;

            int[] buckets = new int[PopulationBarsViewModel.BAR_COUNT];
            int currentBucket = 0;
            double currentUpperBound = min + bucketRange;

            List<Solution> sorted = new List<Solution>(population).OrderByDescending(s => s.Fitness).Reverse().ToList();

            foreach(Solution s in sorted)
            {
                while (s.Fitness > currentUpperBound + EPSILON)
                {
                    currentBucket++;
                    currentUpperBound += bucketRange;
                }
                buckets[currentBucket]++;
            }

            return buckets;
        }
    }

    /// <summary>
    /// ViewModel for managing multiple population bars based on provided run data.
    /// </summary>
    public class PopulationBarsViewModel : PopulationDepictionViewModelBase
    {
        /// <summary>
        /// the number of buckets the population is split into.
        /// </summary>
        public const int BAR_COUNT = 10;

        /// <summary>
        /// Updates the ViewModel with data from a new iteration.
        /// </summary>
        /// <param name="iteration">Iteration data to visualize.</param>
        public override void Update(IterationData iteration)
        {
            Containers.Clear();
            foreach (Population pop in iteration.Populations)
            {
                Containers.Add(new PopulationBar(pop));
            }
            SelectPopulation(SelectedPopulation);
        }
    }
}
