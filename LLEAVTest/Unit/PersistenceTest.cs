using LLEAV.Models;
using LLEAV.Models.Algorithms.MIP;
using LLEAV.Models.FitnessFunction;
using LLEAV.Models.FOSFunction;
using LLEAV.Models.LocalSearchFunction;
using LLEAV.Models.Persistence;
using LLEAV.Models.TerminationCriteria;
using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace LLEAVTest.Unit
{
    public class PersistenceTest
    {
        private const string _directory = "TestDir";


        private readonly ITestOutputHelper _out;
        public PersistenceTest(ITestOutputHelper testOutputHelper)
        {
            _out = testOutputHelper;
        }

        [Fact]
        public void TestSaveAndLoad()
        {
            Directory.CreateDirectory(_directory);

            RunData runData = new RunData();

            runData.NumberOfBits = 39;
            runData.FitnessFunction = new OneMax();
            runData.FOSFunction = new LinkageTreeFOS();

            ATerminationCriteria criteria = new IterationTermination();
            criteria.CreateArgumentFromString("42");

            runData.TerminationCriteria = criteria;

            runData.GrowthFunction = null;
            runData.LocalSearchFunction = new HillClimber();
            runData.Algorithm = new P3();

            Population p = new Population(0);

            Solution s = new Solution()
            {
                Bits = new BitList(39),
            };

            p.Add(s);

            IterationData iteration = new IterationData(p, 13);
            iteration.Iteration = 0;
            Population p2 = p.Clone();


            Solution s2 = new Solution()
            {
                Bits = new BitList(39),
            };
            s2.Bits.Set(1);
            p2.Add(s2);

            IterationData iteration2 = new IterationData(p2, 33);
            iteration2.Iteration = 1;

            runData.Iterations.Add(iteration);
            runData.Iterations.Add(iteration2);

            string path = Path.Combine(_directory, "test.lleav");

            Saver.SaveData(runData, path);


            RunData loaded = Loader.LoadData(path);

            Assert.Equal(39, loaded.NumberOfBits);
         

            Assert.Equal(typeof(Int32), loaded.TerminationCriteria.ArgumentType);
            Assert.IsType(typeof(IterationTermination), loaded.TerminationCriteria);
            Assert.IsType(typeof(OneMax), loaded.FitnessFunction);
            Assert.IsType(typeof(LinkageTreeFOS), loaded.FOSFunction);

            Assert.IsType(typeof(HillClimber), loaded.LocalSearchFunction);
            Assert.IsType(typeof(P3), loaded.Algorithm);
            Assert.Null(loaded.GrowthFunction);

            Assert.True(loaded.Iterations[0].RNGSeed == 13);
            Assert.Equal(0, loaded.Iterations[0].Iteration);
            Assert.Equal(s.Bits, loaded.Iterations[0].Populations[0].Solutions[0].Bits);

            Assert.True(loaded.Iterations[1].RNGSeed == 33);
            Assert.Equal(1, loaded.Iterations[1].Iteration);
            Assert.Equal(s2.Bits, loaded.Iterations[1].Populations[0].Solutions[1].Bits);
        }


    }
}
