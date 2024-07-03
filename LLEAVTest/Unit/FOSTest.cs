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
    public class FOSTest
    {
        private readonly ITestOutputHelper _out;
        public FOSTest(ITestOutputHelper testOutputHelper)
        {
            _out = testOutputHelper;
        }

        [Fact]
        public void TestLinkageTree1()
        {
            BitList b1 = new BitList(6);
            BitList b2 = new BitList(6);
            BitList b3 = new BitList(6);
            BitList b4 = new BitList(6);

            b2.Set(0);
            b2.Set(1);

            b3.Set(2);
            b3.Set(3);

            b4.Set(0);
            b4.Set(4);
            b4.Set(5);

            Solution s1 = new Solution() { Bits = b1 };
            Solution s2 = new Solution() { Bits = b2 };
            Solution s3 = new Solution() { Bits = b3 };
            Solution s4 = new Solution() { Bits = b4 };

            LinkageTreeFOS linkageTree = new LinkageTreeFOS();
            Population p = new Population(0)
            {
                s1
            };

            FOS f1 = linkageTree.CalculateFOS(p, 6);

            Assert.Empty(f1);
         
            p.Add(s2);

            FOS f2 = linkageTree.CalculateFOS(p, 6);

            Assert.True(f2.Contains(new Cluster([0, 1], 6)));
            Assert.True(f2.Contains(new Cluster([2, 3, 4, 5], 6)));


            p.Add(s3);

            FOS f3 = linkageTree.CalculateFOS(p, 6);

            Assert.True(f3.Contains(new Cluster([0, 1], 6)));
            Assert.True(f3.Contains(new Cluster([2, 3], 6)));
            Assert.True(f3.Contains(new Cluster([4, 5], 6)));
            Assert.True(f3.Contains(new Cluster([0, 1, 2, 3], 6)));


            p.Add(s4);

            FOS f4 = linkageTree.CalculateFOS(p, 6);

            Assert.True(f4.Contains(new Cluster([0], 6)));
            Assert.True(f4.Contains(new Cluster([1], 6)));
            Assert.True(f4.Contains(new Cluster([2, 3], 6)));
            Assert.True(f4.Contains(new Cluster([4, 5], 6)));
            Assert.True(f4.Contains(new Cluster([0, 1], 6)));
            Assert.True(f4.Contains(new Cluster([0, 1, 2, 3], 6)));
        }

        [Fact]
        public void TestLinkageTree2()
        {
            BitList b1 = new BitList(6);
            BitList b2 = new BitList(6);
            BitList b3 = new BitList(6);
            BitList b4 = new BitList(6);

            b2.Set(0);
            b2.Set(1);
            b2.Set(2);

            b3.Set(0);

            b4.Set(1);

            Solution s1 = new Solution() { Bits = b1 };
            Solution s2 = new Solution() { Bits = b2 };
            Solution s3 = new Solution() { Bits = b3 };
            Solution s4 = new Solution() { Bits = b4 };


            Population p = new Population(0)
            {
                s1, s2, s3, s4
            };

            LinkageTreeFOS linkageTree = new LinkageTreeFOS();
            FOS f1 = linkageTree.CalculateFOS(p, 6);

            foreach(Cluster c in f1)
            {
                _out.WriteLine(c.ToString());
            }

            Assert.True(f1.Contains(new Cluster([0], 6)));
            Assert.True(f1.Contains(new Cluster([1], 6)));
            Assert.True(f1.Contains(new Cluster([2], 6)));
            Assert.True(f1.Contains(new Cluster([0, 2], 6)));
            Assert.True(f1.Contains(new Cluster([0, 1, 2], 6)));
            Assert.True(f1.Contains(new Cluster([3, 4, 5], 6)));
        }

        [Fact]
        public void TestMarginalProduct()
        {
            BitList b1 = new BitList(6);
            BitList b2 = new BitList(6);

            b2.Set(0);
            b2.Set(1);
            b2.Set(2);
            b2.Set(3);

            Solution s1 = new Solution() { Bits = b1 };
            Solution s2 = new Solution() { Bits = b2 };

            Population p = new Population(0)
            {
                s1, s2,
            };

            MarginalProductFOS marginalProduct = new MarginalProductFOS();
            FOS f1 = marginalProduct.CalculateFOS(p, 6);

            Assert.True(f1.Contains(new Cluster([0,1], 6)));
            Assert.True(f1.Contains(new Cluster([2,3], 6)));
            Assert.True(f1.Contains(new Cluster([4], 6)));
            Assert.True(f1.Contains(new Cluster([5], 6)));
        }

        [Fact]
        public void TestMPVariant()
        {
            BitList b1 = new BitList(6);
            BitList b2 = new BitList(6);

            b2.Set(0);
            b2.Set(1);
            b2.Set(2);
            b2.Set(3);

            Solution s1 = new Solution() { Bits = b1 };
            Solution s2 = new Solution() { Bits = b2 };

            Population p = new Population(0)
            {
                s1, s2,
            };

            MPVariantFOS mpvariant = new MPVariantFOS();
            FOS f1 = mpvariant.CalculateFOS(p, 6);

            Assert.True(f1.Contains(new Cluster([0, 1,2,3], 6)));
            Assert.True(f1.Contains(new Cluster([4,5], 6)));
        }
    }
}
