using DynamicData;
using LLEAV.Models.Algorithms.MIP;
using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NAWVM = LLEAV.ViewModels.Windows.NewAlgorithmWindowViewModel;

namespace LLEAV.Models.Persistence
{
    /// <summary>
    /// Utility class for saving simulation data.
    /// </summary>
    public class Saver
    {
        /// <summary>
        /// Saves rundata to a specified location.
        /// </summary>
        /// <param name="data">The rundata to save.</param>
        /// <param name="path">The location to write the file to.</param>
        public static void SaveData(RunData data, string path)
        {
            bool appending = data.Algorithm is P3 || data.Algorithm is MIP;

            ByteArrayToFile(path, ByteArrayForRunData(data, appending));
        }

        private static byte[] ByteArrayForRunData(RunData data, bool appending)
        {

            List<byte[]> iterationByteRepresentations = new List<byte[]>();

            for (int i = 0; i < data.Iterations.Count; i++)
            {
                if (i == 0 || !appending)
                {
                    iterationByteRepresentations.Add(IterationToByteArray(data.Iterations[i], data.NumberOfBits));
                }
                else
                {
                    iterationByteRepresentations.Add(IterationToByteArray(data.Iterations[i], data.Iterations[i - 1], data.NumberOfBits));
                }
            }

            byte[] terminationArgBytes = data.TerminationCriteria.ConvertArgumentToBytes();
            byte[] fitnessArgBytes = data.FitnessFunction.ConvertArgumentToBytes();

            byte[] bytes = new byte[49 + terminationArgBytes.Length + fitnessArgBytes.Length + iterationByteRepresentations.Sum(a => a.Length)];

            bytes[0] = (byte)(appending ? 1 : 0);

            int index = 1;

            ByteUtil.WriteIntToBuffer(data.NumberOfBits, bytes, index);
            index += 4;

            // Fitness function
            ByteUtil.WriteIntToBuffer(NAWVM.FitnessFunctions.IndexOf(data.FitnessFunction.GetType()), bytes, index);
            index += 4;
            ByteUtil.WriteIntToBuffer(fitnessArgBytes.Length, bytes, index);
            index += 4;
            ByteUtil.WriteByteArrayToBuffer(fitnessArgBytes, bytes, index);
            index += fitnessArgBytes.Length;

            ByteUtil.WriteIntToBuffer(NAWVM.FOSFunctions.IndexOf(data.FOSFunction.GetType()), bytes, index);
            index += 4;

            // Termination criteria
            ByteUtil.WriteIntToBuffer(NAWVM.TerminationCriterias.IndexOf(data.TerminationCriteria.GetType()), bytes, index);
            index += 4;
            ByteUtil.WriteIntToBuffer(terminationArgBytes.Length, bytes, index);
            index += 4;
            ByteUtil.WriteByteArrayToBuffer(terminationArgBytes, bytes, index);
            index += terminationArgBytes.Length;


            ByteUtil.WriteIntToBuffer(NAWVM.Algorithms.IndexOf(data.Algorithm.GetType()), bytes, index);
            index += 4;

            ByteUtil.WriteIntToBuffer(data.LocalSearchFunction != null
                ? NAWVM.LocalSearchFunctions.IndexOf(data.LocalSearchFunction.GetType())
                : -1, bytes, index);
            index += 4;

            ByteUtil.WriteIntToBuffer(data.GrowthFunction != null
                ? NAWVM.GrowthFunctions.IndexOf(data.GrowthFunction.GetType())
                : -1, bytes, index);
            index += 4;

            ByteUtil.WriteIntToBuffer(data.NumberOfSolutions, bytes, index);
            index += 4;

            ByteUtil.WriteIntToBuffer(data.RNGSeed, bytes, index);
            index += 4;

            ByteUtil.WriteIntToBuffer(data.Iterations.Count, bytes, index);
            index += 4;


            foreach (byte[] a in iterationByteRepresentations)
            {
                ByteUtil.WriteByteArrayToBuffer(a, bytes, index);
                index += a.Length;
            }


            return bytes;
        }

        private static byte[] IterationToByteArray(IterationData iteration, int numberOfBitsInSolution)
        {
            int bytesForSolution = (int)Math.Ceiling(numberOfBitsInSolution / 8f);

            List<Solution> solutionsToConvert = new List<Solution>();
            List<int> populationIndices = new List<int>();

            for (int i = 0; i < iteration.Populations.Count; i++)
            {
                for (int j = 0; j < iteration.Populations[i].Solutions.Count; j++)
                {
                    solutionsToConvert.Add(iteration.Populations[i].Solutions[j]);
                    populationIndices.Add(i);
                }
            }

            return SolutionsToByteArray(bytesForSolution, solutionsToConvert, populationIndices, iteration.RNGSeed);
        }


        private static byte[] IterationToByteArray(IterationData iteration, IterationData previous, int numberOfBitsInSolution)
        {
            int bytesForSolution = (int)Math.Ceiling(numberOfBitsInSolution / 8f);

            List<Solution> solutionsToConvert = new List<Solution>();
            List<int> populationIndices = new List<int>();

            for (int i = 0; i < iteration.Populations.Count; i++)
            {
                if (i >= previous.Populations.Count)
                {
                    foreach (Solution s in iteration.Populations[i])
                    {
                        solutionsToConvert.Add(s);
                        populationIndices.Add(i);
                    }

                }
                else
                {
                    for (int j = 0; j < iteration.Populations[i].Solutions.Count; j++)
                    {
                        if (j >= previous.Populations[i].Solutions.Count)
                        {
                            solutionsToConvert.Add(iteration.Populations[i].Solutions[j]);
                            populationIndices.Add(i);
                        }
                    }
                }
            }

            return SolutionsToByteArray(bytesForSolution, solutionsToConvert, populationIndices, iteration.RNGSeed);
        }

        private static byte[] SolutionsToByteArray(int bytesForSolution, List<Solution> solutionsToConvert, List<int> populationIndices, int iterationRNG)
        {
            byte[] bytes = new byte[(bytesForSolution + 12) * solutionsToConvert.Count + 8];

            int index = 0;

            ByteUtil.WriteIntToBuffer(iterationRNG, bytes, index);
            index += 4;

            ByteUtil.WriteIntToBuffer(solutionsToConvert.Count, bytes, index);
            index += 4;

            for (int i = 0; i < solutionsToConvert.Count; i++)
            {
                ByteUtil.WriteIntToBuffer(populationIndices[i], bytes, index);
                index += 4;
                ByteUtil.WriteDoubleToBuffer(solutionsToConvert[i].Fitness, bytes, index);
                index += 8;
                byte[] bitListBytes = solutionsToConvert[i].Bits.ToByteArray();
                ByteUtil.WriteByteArrayToBuffer(bitListBytes, bytes, index);
                index += bytesForSolution;
            }

            return bytes;
        }

        private static bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool IsValidPath(string? path)
        {
            return true;
        }

    }
}
