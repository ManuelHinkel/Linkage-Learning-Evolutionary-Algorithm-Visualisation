using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLEAV.Models.Algorithms;
using LLEAV.Models.FitnessFunction;
using LLEAV.Util;
using LLEAV.ViewModels;

namespace LLEAV.Models.Algorithms.GOM
{
    public class GOMEA : ILinkageLearningAlgorithm
    {
        private GOMEAHistoryTracker? _tracker;

        public Tuple<IterationData, IList<IStateChange>> CalculateIteration(IterationData currentIteration, RunData runData)
        {
            _tracker = new GOMEAHistoryTracker();
            Random random = new Random(currentIteration.RNGSeed);

            IterationData iterationData = currentIteration.Clone();

            Population population = iterationData.Populations[0];                

            IList<Solution> newSolutions = new List<Solution>();

            for(int i = 0; i < runData.NumberOfSolutions; i++)
            {
                newSolutions.Add(GOM(population.Solutions[i], population, population.FOS, runData.FitnessFunction, random));
            }

            population.ClearAndAddAll(
                AlgorithmFunctions.TournamentSelection(newSolutions, runData.NumberOfSolutions, 2, random)
            );


            population.FOS
                = runData.FOSFunction.CalculateFOS(population, runData.NumberOfBits);

            return new Tuple<IterationData, IList<IStateChange>>(iterationData, _tracker.StateChangeHistory);
        }

        
        
       
        private Solution GOM(Solution toMix, Population population, FOS fos, IFitnessFunction fitnessFunction, Random random)
        {
            Solution o = toMix.Clone();

            foreach (Cluster c in fos)
            {
                //_tracker.ChangeFOSCluster(c);
                Solution parent = population.Solutions[random.Next(0, population.Solutions.Count)];
                //_tracker.ChangeDonor(donor);

                Solution temp = Solution.Merge(o, parent, c);

                //_tracker.Merge(temp);

                temp.Fitness = fitnessFunction.Fitness(temp);
                if (temp.Fitness > o.Fitness)
                {
                    o = temp;
                    //_tracker.ApplyCrossover(temp);
                }
            }

            return o;
        }

        public AlgorithmType GetAlgorithmType()
        {
            return AlgorithmType.GOM;
        }

        public Population InitialPopulation(RunData runData, Random random)
        {
            Population initial = new Population(0);

            for (int i = 0; i < runData.NumberOfSolutions; i++)
            {
                var s = Solution.RandomSolution(runData.NumberOfBits, random);
                s.Fitness = runData.FitnessFunction.Fitness(s);
                initial.Add(s);
            }

            initial.ClearAndAddAll(AlgorithmFunctions.TournamentSelection(initial.Solutions, runData.NumberOfSolutions, 2, random));
            
            foreach( Solution s in initial)
            {
                s.Fitness = runData.FitnessFunction.Fitness(s);
            }

            initial.FOS = runData.FOSFunction.CalculateFOS(initial, runData.NumberOfBits);

            return initial;
        }
    }
}
