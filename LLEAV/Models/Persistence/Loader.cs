using LLEAV.Models.Algorithms;
using LLEAV.Models.FitnessFunction;
using LLEAV.Models.FOSFunction;
using LLEAV.Models.GrowthFunction;
using LLEAV.Models.LocalSearchFunction;
using LLEAV.Models.TerminationCriteria;
using LLEAV.Util;
using LLEAV.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAWVM = LLEAV.ViewModels.Windows.NewAlgorithmWindowViewModel;

namespace LLEAV.Models.Persistence
{
    public class Loader
    {
        public static RunData LoadData(string path) 
        {
            RunData data = Convert(File.ReadAllBytes(path));
            data.FilePath = path;
            return data; 
        }

        private static RunData Convert(byte[] bytes)
        {
            bool appending = (bytes[0] & 0x01) > 0;

            int index = 1;

            int numberOfBits = BitConverter.ToInt32(bytes, index);
            index += 4;

            // Fitness function
            int fitnessFunctionIndex = BitConverter.ToInt32(bytes, index);
            index += 4;
            int fitnessArgLength = BitConverter.ToInt32(bytes, index);
            index += 4;
            byte[] fitnessArg = bytes.Skip(index).Take(fitnessArgLength).ToArray();
            index += fitnessArgLength;

            int fosFunction = BitConverter.ToInt32(bytes, index);
            index += 4;

            // Termination criteria
            int terminationCriteriaIndex = BitConverter.ToInt32(bytes, index);
            index += 4;
            int terminationCriteriaArgLength = BitConverter.ToInt32(bytes, index);
            index += 4;
            byte[] terminationArg = bytes.Skip(index).Take(terminationCriteriaArgLength).ToArray();
            index += terminationCriteriaArgLength;

            int algorithm = BitConverter.ToInt32(bytes, index);
            index += 4;

            int localSearchFunction = BitConverter.ToInt32(bytes, index);
            index += 4;

            int growthFunction = BitConverter.ToInt32(bytes, index);
            index += 4;

            int numberOfSolutions = BitConverter.ToInt32(bytes, index);
            index += 4;

            int runRNG = BitConverter.ToInt32(bytes, index);
            index += 4;

            int iterationCount = BitConverter.ToInt32(bytes, index);
            index += 4;

            List<IterationData> iterationData = new List<IterationData>();

            for (int i = 0; i < iterationCount; i++)
            {
                Tuple<IterationData, int> result;
                if (i == 0 || !appending)
                {
                    result = ConvertIteration(bytes, index, numberOfBits);
                }
                else
                {
                    result = ConvertIteration(bytes, index, numberOfBits, iterationData[i - 1]);
                }
                result.Item1.Iteration = i;
                iterationData.Add(result.Item1);
                index = result.Item2;
            }

            ATerminationCriteria terminationCriteria = Activator.CreateInstance(NAWVM.TerminationCriterias[terminationCriteriaIndex])
                   as ATerminationCriteria;
            terminationCriteria.CreateArgumentFromBytes(terminationArg);

            AFitnessFunction fitnessFunction = Activator.CreateInstance(NAWVM.FitnessFunctions[fitnessFunctionIndex])
                   as AFitnessFunction;
            fitnessFunction.CreateArgumentFromBytes(fitnessArg);

            RunData newRunData = new RunData
            {
                Algorithm = Activator.CreateInstance(NAWVM.Algorithms[algorithm]) as ALinkageLearningAlgorithm,
                FOSFunction = Activator.CreateInstance(NAWVM.FOSFunctions[fosFunction]) as AFOSFunction,
                FitnessFunction = fitnessFunction,
                TerminationCriteria = terminationCriteria,
                NumberOfBits = numberOfBits,
                NumberOfSolutions = numberOfSolutions,
                RNGSeed = runRNG,
            };

            if (algorithm == 0 || algorithm == 1)
            {
                newRunData.LocalSearchFunction = Activator.CreateInstance(NAWVM.LocalSearchFunctions[localSearchFunction]) as ALocalSearchFunction;
            }

            if (algorithm == 0)
            {
                newRunData.GrowthFunction = Activator.CreateInstance(NAWVM.GrowthFunctions[growthFunction]) as AGrowthFunction;
            }

            newRunData.Iterations = iterationData;

            foreach (IterationData iteration in iterationData)
            {
                iteration.LastIteration = newRunData.TerminationCriteria.ShouldTerminate(iteration);

                foreach (Population p in iteration.Populations)
                {
                    p.FOS = newRunData.FOSFunction.CalculateFOS(p, numberOfBits);
                }
            }

            return newRunData;
        }

        private static Tuple<IterationData, int> ConvertIteration(byte[] bytes, int index, int numberOfBits)
        {
            return ConvertIteration(bytes, index, numberOfBits, new IterationData(new Population(0), 0));
        }

        private static Tuple<IterationData, int> ConvertIteration(byte[] bytes, int index, int numberOfBits, IterationData previous)
        {
            int bytesForSolution = (int)Math.Ceiling(numberOfBits / 8f);

            int iterationRng = BitConverter.ToInt32(bytes, index);
            index += 4;

            int solutionCount = BitConverter.ToInt32(bytes, index);
            index += 4;

            IterationData iteration = previous.Clone();

            iteration.RNGSeed = iterationRng;

            for (int i = 0; i < solutionCount; i++)
            {
                int populationIndex = BitConverter.ToInt32(bytes, index);
                index += 4;

                double fitness = BitConverter.ToDouble(bytes, index);
                index += 8;

                byte[] bitList = bytes.Skip(index).Take(bytesForSolution).ToArray();
                index += bytesForSolution;

                while (iteration.Populations.Count() <= populationIndex)
                {
                    iteration.Populations.Add(new Population(iteration.Populations.Count));
                }

                Solution s = new Solution()
                {
                    Fitness = fitness,
                    Bits = new BitList(numberOfBits, bitList),
                };

                iteration.Populations[populationIndex].Add(s);
            }

            return new Tuple<IterationData, int>(iteration, index);
        }
    }
}
