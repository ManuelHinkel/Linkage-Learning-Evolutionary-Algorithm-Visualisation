using LLEAV.Util;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models
{
    public class Population : IEnumerable<Solution>
    {
        public Population? Previous { get; set; }
        public IList<Solution> Solutions { get; set; } = new List<Solution>();
        public FOS? FOS { get; set; }

        public double MaximumFitness { get; private set; } = int.MinValue;

        public double MinimumFitness { get; private set; } = int.MaxValue;

        public double AverageFitness { get; private set; }
        
        public double MedianFitness { get; private set; }

        public int PyramidIndex { get; private set; }

        public Population(int pyramidIndex)
        {
            PyramidIndex = pyramidIndex;
        }

        public void Add(Solution solution)
        {
            Solutions.Add(solution);
            if (solution.Fitness > MaximumFitness)
            {
                MaximumFitness = solution.Fitness;
            }

            if (solution.Fitness < MinimumFitness)
            {
                MinimumFitness = solution.Fitness;
            }
            CalculateAverageFitness();
            CalculateMedianFitness();

        }

        public void ClearAndAddAll(IList<Solution> solutions)
        {
            Solutions.Clear();
            foreach(Solution solution in solutions)
            {
                Add(solution);
            }
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

        public IEnumerator<Solution> GetEnumerator()
        {
            return Solutions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            string s = "";
            foreach (Solution solution in Solutions)
            {
                s += solution.ToString() + "\n";
            }
            return s.Trim();
        }

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
