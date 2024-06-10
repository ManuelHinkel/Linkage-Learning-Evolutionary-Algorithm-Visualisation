using LLEAV.Models;
using LLEAV.Models.FitnessFunction;
using LLEAV.Models.TerminationCriteria;
using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAVTest.Unit
{
    public class TerminationCriteriaTest
    {
        [Fact]
        public void TestIterationTermination()
        {
            IterationData iteration = new IterationData(new Population(0), 0);


            IterationTermination iterationTermination = new IterationTermination();
            iterationTermination.CreateArgumentFromString("10");

            iteration.Iteration = 0;

            Assert.False(iterationTermination.ShouldTerminate(iteration));

            iteration.Iteration = 9;
            Assert.False(iterationTermination.ShouldTerminate(iteration));

            iteration.Iteration = 10;
            Assert.True(iterationTermination.ShouldTerminate(iteration));
        }

        [Fact]
        public void TestFitnessTermination()
        {
            BitList b = new BitList(10);

            b.Set(0);
            b.Set(1);

            Solution s = new Solution()
            {
                Bits = b,
            };

            Population p = new Population(0)
            {
                s
            };

            IterationData iteration = new IterationData(p, 0);

            LeadingOnes oneMax = new LeadingOnes();

            s.Fitness = oneMax.Fitness(s);

            FitnessTermination fitnessTermination = new FitnessTermination();
            fitnessTermination.CreateArgumentFromString("3.0");

            Assert.False(fitnessTermination.ShouldTerminate(iteration));

            b.Set(2);
            s.Fitness = oneMax.Fitness(s);
            
            Assert.True(fitnessTermination.ShouldTerminate(iteration));
        }
    }
}
