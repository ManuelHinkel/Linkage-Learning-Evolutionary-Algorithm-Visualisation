using LLEAV.Models;
using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.FOSFunction
{
    public class MarginalProductFOS : AFOSFunction
    {
        public override string Depiction { get; } = "Marginal Product Structure";
        public override FOS CalculateFOS(Population population, int numberOfBits)
        {
            List<Cluster> output = new List<Cluster>();
            for (int i = 0; i < numberOfBits; i++)
            {
                output.Add(new Cluster([i], numberOfBits));
            }

            bool mdlDecreased = true;
            while (mdlDecreased)
            {
                mdlDecreased = false;
                Tuple<Cluster, Cluster, double> currentBest = new Tuple<Cluster, Cluster, double>(null, null, 0);
                for (int i = 0; i < output.Count() - 1; i++)
                {
                    for (int j = i + 1; j < output.Count(); j++)
                    {
                        Cluster cI = output.ElementAt(i);
                        Cluster cJ = output.ElementAt(j);

                        double currentDecrease = AlgorithmFunctions.MDLDecrease(population, cI, cJ, numberOfBits);

                        if (currentDecrease > currentBest.Item3) 
                        {
                            currentBest = new Tuple<Cluster, Cluster, double>(cI, cJ, currentDecrease);
                            mdlDecreased = true;
                        }
                    }
                }
                if (mdlDecreased)
                {
                    output.Remove(currentBest.Item1);
                    output.Remove(currentBest.Item2);
                    output.Add(currentBest.Item1.Union(currentBest.Item2));
                }
            }

            return new FOS(output, FOSType.MARGINAL_PRODUCT);
        }
    }
}
