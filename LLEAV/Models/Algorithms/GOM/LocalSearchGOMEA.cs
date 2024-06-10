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
    public class LocalSearchGOMEA : ALinkageLearningAlgorithm
    {
        public override string Depiction { get; } = "GOMEA (Local Search)";

        public override AlgorithmType AlgorithmType { get; } = AlgorithmType.GOM;
        public override bool ShowLocalSearchFunction { get; } = true;
        public override bool ShowGrowthFunction { get; }
        public override bool ShowPopulationSize { get; } = true;

        private GOMEA _gomea = new GOMEA();

        public override Tuple<IterationData, IList<IStateChange>> CalculateIteration(IterationData currentIteration, RunData runData)
        {
            return _gomea.CalculateIteration(currentIteration, runData);
        }

        public override Population InitialPopulation(RunData runData, Random random)
        {
            Population initial = new Population(0);

            for (int i = 0; i < runData.NumberOfSolutions; i++)
            {
                var s = Solution.RandomSolution(runData.NumberOfBits, random);
                s = runData.LocalSearchFunction.Execute(s, runData.FitnessFunction, random);
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
