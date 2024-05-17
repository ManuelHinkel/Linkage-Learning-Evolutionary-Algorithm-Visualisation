using LLEAV.ViewModels;
using System;
using System.Collections.Generic;

namespace LLEAV.Models.Algorithms.ROM
{
    public class ROMEA : ILinkageLearningAlgorithm
    {
        public Tuple<IterationData, IList<IStateChange>> CalculateIteration(IterationData currentIteration, RunData runData)
        {
            throw new NotImplementedException();
        }

        public AlgorithmType GetAlgorithmType()
        {
            return AlgorithmType.ROM;
        }

        public Population InitialPopulation(RunData runData, Random random)
        {
            Population initial = new Population(0);

            for(int i = 0; i < runData.NumberOfSolutions; i++)
            {
                initial.Add(Solution.RandomSolution(runData.NumberOfBits, random));
            }

            // initial.TournamentSelection

            return initial;
        }

 
    }
}
