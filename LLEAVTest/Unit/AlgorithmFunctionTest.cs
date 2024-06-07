using LLEAV.Models;
using LLEAV.Models.FOSFunction;
using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace LLEAVTest.Unit
{
    public class AlgorithmFunctionTest
    {

        private readonly ITestOutputHelper _out;
        public AlgorithmFunctionTest(ITestOutputHelper testOutputHelper)
        {
            _out = testOutputHelper;
        }

        [Fact]
        public void TestP()
        {
            BitList b1 = new BitList(8);
            BitList b2 = !b1;
            BitList b3 = new BitList(8);
            BitList b4 = new BitList(8);

            b1.Set(2);
            b1.Set(3);

            b3.Set(2);
            b3.Set(6);

            b4.Set(4);
            b4.Set(5);

            Solution s1 = new Solution() { Bits = b1 };
            Solution s2 = new Solution() { Bits = b2 };
            Solution s3 = new Solution() { Bits = b3 };
            Solution s4 = new Solution() { Bits = b4 };

            Population p = new Population(0)
            {
                s1, s2, s3, s4
            };

            Cluster c = new Cluster([2, 3], 8);

            Assert.Equal(0.25, AlgorithmFunctions.P(p, c, c.PossibleStrings().ElementAt(0)));
            Assert.Equal(0, AlgorithmFunctions.P(p, c, c.PossibleStrings().ElementAt(1)));
            Assert.Equal(0.25, AlgorithmFunctions.P(p, c, c.PossibleStrings().ElementAt(2)));
            Assert.Equal(0.5, AlgorithmFunctions.P(p, c, c.PossibleStrings().ElementAt(3)));

        }

        [Fact]
        public void TestH()
        {
            BitList b1 = new BitList(8);
            BitList b2 = !b1;
            BitList b3 = new BitList(8);
            BitList b4 = new BitList(8);

            b1.Set(2);
            b1.Set(3);

            b3.Set(2);
            b3.Set(6);

            b4.Set(4);
            b4.Set(5);


            Solution s1 = new Solution() { Bits = b1 };
            Solution s2 = new Solution() { Bits = b2 };
            Solution s3 = new Solution() { Bits = b3 };
            Solution s4 = new Solution() { Bits = b4 };

            Population p = new Population(0)
            {
                s1, s2, s3, s4
            };

            Cluster c = new Cluster([2, 3], 8);

            Assert.Equal(0.45154499349, AlgorithmFunctions.H(p, c), 0.001);

        }

        [Fact]
        public void TestMDLDecrease()
        {
            BitList b1 = new BitList(8);
            BitList b2 = new BitList(8);

            b2.Set(0);
            b2.Set(1);
            b2.Set(2);

            Solution s1 = new Solution() { Bits = b1 };
            Solution s2 = new Solution() { Bits = b2 };

            Population p = new Population(0)
            {
                s1, s2,
            };

            Cluster c1 = new Cluster([0, 1], 8);
            Cluster c2 = new Cluster([2], 8);
            Cluster c3 = new Cluster([3], 8);


            List<Cluster> output = new List<Cluster>();
            for (int i = 2; i < 8; i++)
            {
                output.Add(new Cluster([i], 8));
            }


            output.Add(c1);

            double mdlBefore = MDL2(p, output, 8);

            _out.WriteLine(mdlBefore.ToString());

            output.RemoveAt(output.Count - 1);
            output.RemoveAt(0);
            output.Add(c1.Union(c2));

            double mdlMerge3 = MDL2(p, output, 8);

            _out.WriteLine(mdlMerge3.ToString());

            output.RemoveAt(output.Count - 1);
            output.RemoveAt(0);
            output.Add(c1);
            output.Add(c2.Union(c3));

            double mdlMerge2 = MDL2(p, output, 8);
            _out.WriteLine(mdlMerge2.ToString());

            Assert.Equal(mdlBefore - mdlMerge3, AlgorithmFunctions.MDLDecrease(p, c1, c2, 8), 0.001);
            Assert.Equal(mdlBefore - mdlMerge2, AlgorithmFunctions.MDLDecrease(p, c2, c3, 8), 0.001);

        }

        private double MDL1(Population p, IList<Cluster> clusters, int numberOFBits)
        {
            double firstSum = 0;
            double secondSum = 0;

            foreach(Cluster c in clusters)
            {
                firstSum += AlgorithmFunctions.H(p, c);
                secondSum += (1 << c.Count());
            }
            return numberOFBits * firstSum + secondSum - 1;
        }
        private double MDL2(Population p, IList<Cluster> clusters, int numberOFBits)
        {
            double firstSum = 0;
            double secondSum = 0;

            foreach (Cluster c in clusters)
            {
                firstSum += AlgorithmFunctions.H(p, c);
                secondSum += (1 << c.Count()) - 1.0;
            }
            return numberOFBits * firstSum + secondSum;
        }

    }
}
