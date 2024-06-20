using DynamicData;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LLEAV.Models
{
    /// <summary>
    /// Represents a population of solutions in an evolutionary algorithm.
    /// </summary>
    public class Population : IEnumerable<Solution>
    {
        /// <summary>
        /// Gets or sets the previous population, on which this population build on.
        /// </summary>
        public Population? Previous { get; set; }

        /// <summary>
        /// Gets or sets the list of solutions in the population.
        /// </summary>
        public IList<Solution> Solutions { get; set; } = new List<Solution>();

        /// <summary>
        /// Gets or sets the FOS associated with the population.
        /// </summary>
        public FOS? FOS { get; set; }

        /// <summary>
        /// Gets the maximum fitness value in the population.
        /// </summary>
        public double MaximumFitness { get; private set; } = double.NaN;

        /// <summary>
        /// Gets the minimum fitness value in the population.
        /// </summary>
        public double MinimumFitness { get; private set; } = double.NaN;

        /// <summary>
        /// Gets the average fitness value in the population.
        /// </summary>
        public double AverageFitness { get; private set; } = double.NaN;

        /// <summary>
        /// Gets the median fitness value in the population.
        /// </summary>
        public double MedianFitness { get; private set; } = double.NaN;

        /// <summary>
        /// Gets the index of the pyramid the population belongs to.
        /// </summary>
        public int PyramidIndex { get; private set; }

        /// <summary>
        /// Initializes a new instance of the population with the specified pyramid index.
        /// </summary>
        /// <param name="pyramidIndex">The index of the pyramid the population belongs to.</param>
        public Population(int pyramidIndex)
        {
            PyramidIndex = pyramidIndex;
        }

        /// <summary>
        /// Adds a solution to the population and updates fitness statistics.
        /// </summary>
        /// <param name="solution">The solution to add to the population.</param>
        public void Add(Solution solution)
        {
            Solutions.Add(solution);
            CalculateAverageFitness();
            CalculateMedianFitness();
            CalculateMaximumFitness();
            CalculateMinimumFitness();
        }

        /// <summary>
        /// Clears the population and adds all solutions from the specified list, updating fitness statistics.
        /// </summary>
        /// <param name="solutions">The list of solutions to add to the population.</param>
        public void ClearAndAddAll(IList<Solution> solutions)
        {
            Solutions.Clear();
            Solutions.AddRange(solutions);

            CalculateAverageFitness();
            CalculateMedianFitness();
            CalculateMaximumFitness();
            CalculateMinimumFitness();
        }

        private void CalculateMinimumFitness()
        {
            MinimumFitness = Solutions.Min(s => s.Fitness);
        }

        private void CalculateMaximumFitness()
        {
            MaximumFitness = Solutions.Max(s => s.Fitness);
        }

        private void CalculateAverageFitness()
        {
            AverageFitness = Solutions.Average(s => s.Fitness);
        }

        private void CalculateMedianFitness()
        {
            List<Solution> list = new List<Solution>(Solutions).OrderByDescending(s => s.Fitness).ToList();
            MedianFitness = list[list.Count / 2].Fitness;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the solutions in the population.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the solutions in the population.</returns>
        public IEnumerator<Solution> GetEnumerator()
        {
            return Solutions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns a string representation of the population.
        /// </summary>
        /// <returns>A string that represents the population.</returns>
        public override string ToString()
        {
            string s = "";
            foreach (Solution solution in Solutions)
            {
                s += solution.ToString() + "\n";
            }
            return s.Trim();
        }

        /// <summary>
        /// Creates a deep copy of the current population.
        /// </summary>
        /// <returns>A new population that is a copy of the current population.</returns>
        public Population Clone()
        {
            Population clone = new Population(PyramidIndex);
            clone.FOS = FOS;
            clone.Solutions = new List<Solution>(Solutions);
            clone.Previous = this;
            clone.MedianFitness = MedianFitness;
            clone.MaximumFitness = MaximumFitness;
            clone.AverageFitness = AverageFitness;
            clone.MinimumFitness = MinimumFitness;
            return clone;
        }
    }
}
