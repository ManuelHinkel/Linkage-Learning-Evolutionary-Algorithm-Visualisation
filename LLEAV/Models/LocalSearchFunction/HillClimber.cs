using LLEAV.Models.FitnessFunction;
using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.LocalSearchFunction
{
    public class HillClimber : ILocalSearchFunction
    {
        public Solution Execute(Solution solution, IFitnessFunction fitnessFunction, Random random)
        {
            IEnumerable<int> options = Enumerable.Range(0, solution.Bits.NumberBits);
            HashSet<int> tried = new HashSet<int>();
            Solution copy = solution.Clone();

            List<int> shuffled = options.ToList();
            shuffled.Shuffle(random);

            copy.Fitness = fitnessFunction.Fitness(solution);
            while (tried.Count < options.Count())
            {
                foreach (int index in shuffled)
                {
                    if (!tried.Contains(index))
                    {
                        copy.Bits.Flip(index);

                        double newFitness = fitnessFunction.Fitness(copy);
                        if (newFitness > copy.Fitness)
                        {
                            copy.Fitness = newFitness;
                            tried.Clear();
                        }
                        else
                        {
                            copy.Bits.Flip(index);
                        }
                        tried.Add(index);
                    }
                }
            }
            return copy;
        }

        public Solution Execute(Solution solution, IFitnessFunction fitnessFunction)
        {
            return Execute(solution, fitnessFunction, new Random());
        }
    }
}
