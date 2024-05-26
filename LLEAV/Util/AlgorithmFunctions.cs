using LLEAV.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Util
{
    public class AlgorithmFunctions
    {
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
                if (samplePopulation.Count <  s)
                {
                    samplePopulation.AddRange(used);
                    used.Clear();
                }

                List<Solution> tournament = new List<Solution>();
                for(int k = 0; k < s; k++)
                {
                    Solution sampled = samplePopulation[random.Next(0, samplePopulation.Count())];
                    tournament.Add(sampled);
                    used.Add(sampled);
                    samplePopulation.Remove(sampled);
                }

                Solution best = tournament[0];

                for (int j = 1; j < s; j++)
                {
                    if (tournament[j].Fitness >  best.Fitness)
                    {
                        best = tournament[j];
                    }
                }
                selected.Add(best);
            }

            return selected;
        }
        public static double MDLDecrease(Population population, Cluster c1, Cluster c2, int numberBits)
        {
            Cluster union = c1.Union(c2);
            return numberBits * (H(population, c1) + H(population, c1) - H(population, union))
                + ((1 << c1.Count()) + (1 << c2.Count()) - (1 << union.Count()) - 1);
        }

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

                    if (dValue < currentMin.Item3)                     {
                        currentMin = new Tuple<Cluster, Cluster, double>(cI, cJ, dValue);
                    }
                }
            }
            return currentMin;
        }

        public static double D(Population population, Cluster c1, Cluster c2)
        {
            if (c1.Count() == 0 || c1.Count() == 0)
            {
                throw new ArgumentException("clusters don't have any members");
            }
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

        public static double H(Population population, Cluster cluster)
        {
            double h = 0;

            foreach (BitList possibleString in cluster.PossibleStrings())
            {
                double p = P(population, cluster, possibleString);
                if (p != 0)
                {
                    h += p * Math.Log(p);
                }
            }

            return -h;
        }

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
