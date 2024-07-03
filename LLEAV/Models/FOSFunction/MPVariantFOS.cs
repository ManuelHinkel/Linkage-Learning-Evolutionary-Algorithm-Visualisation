using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.FOSFunction
{
    public class MPVariantFOS: AFOSFunction
    {
        public override string Depiction { get; } = "Marginal Product Structure Variant";
        public override FOS CalculateFOS(Population population, int numberOfBits)
        {
            List<Cluster> output = new List<Cluster>();
            for (int i = 0; i < numberOfBits; i++)
            {
                output.Add(new Cluster([i], numberOfBits));
            }

            bool decreased = true;
            while (decreased)
            {
                decreased = false;
                Tuple<Cluster, Cluster, double> min = AlgorithmFunctions.MinimizingClusters(population, output);

                if (Math.Abs(min.Item3) <= 0.001)
                {
                    Cluster union = min.Item1.Union(min.Item2);
                    output.Add(union);
                    output.Remove(min.Item1);
                    output.Remove(min.Item2);
                    decreased = true;
                }
            }

            return new FOS(output, FOSType.MARGINAL_PRODUCT);
        }
    }
}

