using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LLEAV.Models.FOSFunction
{
    public class LinkageTreeFOS : IFOSFunction
    {
        public FOS CalculateFOS(Population population, int numberOfBits)
        {
            if (population.Count() == 0) throw new Exception("Population can't be empty");

            List<Cluster> unmerged = new List<Cluster>();
            List<Cluster> output = new List<Cluster>();
            for (int i = 0; i < numberOfBits; i++)
            {
                unmerged.Add(new Cluster([i], numberOfBits));
                output.Add(unmerged[i]);
            }

            while (unmerged.Count > 1)
            {
                Tuple<Cluster, Cluster, double> min = AlgorithmFunctions.MinimizingClusters(population, unmerged);

                unmerged.Remove(min.Item1);
                unmerged.Remove(min.Item2);

                Cluster union = min.Item1.Union(min.Item2);
                unmerged.Add(union);
                output.Add(union);

                if (min.Item3 == 0)
                {
                    output.Remove(min.Item1);
                    output.Remove(min.Item2);
                }
            }

            // Remove cluster containing all bits
            output.Remove(new Cluster(Enumerable.Range(0, numberOfBits).ToList(), numberOfBits));

            return new FOS(output, FOSType.LINKAGE_TREE);
        }
    }
}
