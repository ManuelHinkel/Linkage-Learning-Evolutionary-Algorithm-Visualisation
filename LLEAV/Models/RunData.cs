using DynamicData;
using LLEAV.Models.Algorithms;
using LLEAV.Models.FitnessFunction;
using LLEAV.Models.FOSFunction;
using LLEAV.Models.GrowthFunction;
using LLEAV.Models.LocalSearchFunction;
using LLEAV.Models.TerminationCriteria;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LLEAV.Models
{
    public class IterationData
    {
        public IList<Population> Populations { get; set; }
        public int Iteration { get; set; } = -1;
        public bool LastIteration { get; set; }
        public int RNGSeed { get; set; }


        private IterationData() { }
        public IterationData(Population initialPopulation, int rngSeed)
        {
            Populations = [initialPopulation];
            RNGSeed = rngSeed;
        }

        public IterationData Clone()
        {
            IterationData clone = new IterationData()
            {
                Iteration = this.Iteration,
                LastIteration = this.LastIteration,
                RNGSeed = this.RNGSeed,
                Populations = new List<Population>(Populations.Select(p => p.Clone())),
            };

            return clone;
        }
    }
    public class RunData
    {
        public IList<IterationData> Iterations { get; set; } = new List<IterationData>();
        public ALinkageLearningAlgorithm Algorithm { get; set; }
        public AFitnessFunction FitnessFunction { get; set; }
        public ALocalSearchFunction LocalSearchFunction { get; set; }
        public AFOSFunction FOSFunction { get; set; }

        public ATerminationCriteria TerminationCriteria { get; set; } 
        public AGrowthFunction GrowthFunction { get; set; }
        public int NumberOfBits { get; set; }
        public int NumberOfSolutions { get; set; }

        public int RNGSeed { get; set; }
        public string? FilePath { get; set; }

    }
}
