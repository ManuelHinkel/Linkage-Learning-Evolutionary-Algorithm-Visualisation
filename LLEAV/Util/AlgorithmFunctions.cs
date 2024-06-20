using LLEAV.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LLEAV.Util
{
    /// <summary>
    /// Provides static methods for various algorithmic functions used in evolutionary algorithms.
    /// </summary>
    public class AlgorithmFunctions
    {
        /// <summary>
        /// Performs tournament selection on a list of solutions.
        /// </summary>
        /// <param name="solutions">The list of solutions.</param>
        /// <param name="n">The number of solutions to select.</param>
        /// <param name="s">The tournament size.</param>
        /// <param name="random">The random number generator.</param>
        /// <returns>A list of selected solutions.</returns>
        /// <exception cref="Exception">
        /// Thrown if the solutions list is empty, if more solutions are requested than available, or if the tournament size is invalid.
        /// </exception>
        public static IList<Solution> TournamentSelection(IList<Solution> solutions, int n, int s, Random random)
        {
            if (solutions.Count == 0) throw new Exception("Solutions must not be emtpy");
            if (solutions.Count < n) throw new Exception("Can't select more solutions than population contains.");
            if (solutions.Count < s || n < s) throw new Exception("Tournament size invalid.");

            List<Solution> samplePopulation = new List<Solution>(solutions);
            List<Solution> selected = new List<Solution>();

            List<Solution> used = new List<Solution>();
            for (int i = 0; i < n; i++)
            {
                if (samplePopulation.Count < s)
                {
                    samplePopulation.AddRange(used);
                    used.Clear();
                }

                List<Solution> tournament = new List<Solution>();
                for (int k = 0; k < s; k++)
                {
                    Solution sampled = samplePopulation[random.Next(0, samplePopulation.Count())];
                    tournament.Add(sampled);
                    used.Add(sampled);
                    samplePopulation.Remove(sampled);
                }

                Solution best = tournament[0];

                for (int j = 1; j < s; j++)
                {
                    if (tournament[j].Fitness > best.Fitness)
                    {
                        best = tournament[j];
                    }
                }
                selected.Add(best);
            }

            return selected;
        }

        /// <summary>
        /// Calculates the Minimum Description Length (MDL) decrease for two clusters.
        /// </summary>
        /// <param name="population">The population of solutions.</param>
        /// <param name="c1">The first cluster.</param>
        /// <param name="c2">The second cluster.</param>
        /// <param name="numberBits">The number of bits.</param>
        /// <returns>The MDL decrease value.</returns>
        public static double MDLDecrease(Population population, Cluster c1, Cluster c2, int numberBits)
        {
            Cluster union = c1.Union(c2);
            return numberBits * (H(population, c1) + H(population, c2) - H(population, union))
                + ((1 << c1.Count()) + (1 << c2.Count()) - (1 << union.Count()) - 1);
        }

        /// <summary>
        /// Finds the pair of clusters that minimizes the distance function D.
        /// </summary>
        /// <param name="population">The population of solutions.</param>
        /// <param name="clusters">The enumerable of clusters.</param>
        /// <returns>A tuple containing the two clusters and the minimum distance value.</returns>
        public static Tuple<Cluster, Cluster, double> MinimizingClusters(Population population, IEnumerable<Cluster> clusters)
        {
            Tuple<Cluster, Cluster, double> currentMin = new Tuple<Cluster, Cluster, double>(null, null, double.PositiveInfinity);
            for (int i = 0; i < clusters.Count() - 1; i++)
            {
                for (int j = i + 1; j < clusters.Count(); j++)
                {
                    Cluster cI = clusters.ElementAt(i);
                    Cluster cJ = clusters.ElementAt(j);
                    double dValue = D(population, cI, cJ);

                    if (dValue < currentMin.Item3)
                    {
                        currentMin = new Tuple<Cluster, Cluster, double>(cI, cJ, dValue);
                    }
                }
            }
            return currentMin;
        }

        /// <summary>
        /// Calculates the distance function D for two clusters.
        /// </summary>
        /// <param name="population">The population of solutions.</param>
        /// <param name="c1">The first cluster.</param>
        /// <param name="c2">The second cluster.</param>
        /// <returns>The distance value.</returns>
        public static double D(Population population, Cluster c1, Cluster c2)
        {
            Debug.Assert(c1.Count() > 0 && c2.Count() > 0);

            double factor = 1.0 / (c1.Count() * c2.Count());

            double sum = 0;

            foreach (Cluster a in c1)
            {
                foreach (Cluster b in c2)
                {
                    double hTop = H(population, a) + H(population, b);
                    double hBottom = H(population, a.Union(b));

                    if (hBottom != 0)
                    {
                        sum += 2 - hTop / hBottom;
                    }
                }
            }
            return factor * sum;
        }

        /// <summary>
        /// Calculates the entropy H for a given cluster in a population.
        /// </summary>
        /// <param name="population">The population of solutions.</param>
        /// <param name="cluster">The cluster.</param>
        /// <returns>The entropy value.</returns>
        public static double H(Population population, Cluster cluster)
        {
            double h = 0;

            foreach (BitList possibleString in cluster.PossibleStrings())
            {
                double p = P(population, cluster, possibleString);
                if (p != 0)
                {
                    h += p * Math.Log10(p);
                }
            }

            return -h;
        }

        /// <summary>
        /// Calculates the probability P of a possible string in a cluster within a population.
        /// </summary>
        /// <param name="population">The population of solutions.</param>
        /// <param name="cluster">The cluster.</param>
        /// <param name="possibleString">The possible string represented by a BitList.</param>
        /// <returns>The probability value.</returns>
        public static double P(Population population, Cluster cluster, BitList possibleString)
        {
            int matches = 0;
            BitList clusterMask = cluster.Mask;
            foreach (Solution solution in population)
            {
                if ((solution.Bits & clusterMask).Equals(possibleString))
                {
                    matches++;
                }
            }

            return (double)matches / population.Count();
        }
    }
}
