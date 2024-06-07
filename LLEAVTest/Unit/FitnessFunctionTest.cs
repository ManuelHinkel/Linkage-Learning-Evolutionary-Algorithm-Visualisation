using LLEAV.Models;
using LLEAV.Models.FitnessFunction;
using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAVTest.Unit
{
    public class FitnessFunctionTest
    {
        [Fact]
        public void TestOneMax()
        {
            BitList b = new BitList(10);
            b.Set(0);
            b.Set(1);

            Solution s = new Solution()
            {
                Bits = b,
            };

            OneMax oneMax = new OneMax();

            Assert.Equal(2.0, oneMax.Fitness(s));

            b.Set(3);
            b.Set(4);
            b.Set(5);

            Assert.Equal(2.0, oneMax.Fitness(s));

            b.Set(2);

            Assert.Equal(6.0, oneMax.Fitness(s));

            b.Set(0, false);

            Assert.Equal(0, oneMax.Fitness(s));
        }


        [Fact]
        public void TestDeceptiveTrap()
        {
            BitList b = new BitList(14);

            Solution s = new Solution()
            {
                Bits = b,
            };

            DeceptiveTrap deceptiveTrap = new DeceptiveTrap();

            Assert.Equal(12.0, deceptiveTrap.Fitness(s));

            b.Set(0);
            b.Set(1);
            b.Set(2);
            b.Set(3);
            b.Set(4);
            b.Set(5);

            b.Set(7);
            b.Set(8);
            b.Set(9);
            b.Set(10);
            b.Set(11);
            b.Set(12);

            Assert.Equal(0, deceptiveTrap.Fitness(s));

            b.Set(6);
            b.Set(13);

            Assert.Equal(14.0, deceptiveTrap.Fitness(s));

        }

        [Fact]
        public void TestHIFF()
        {
            BitList b = new BitList(8);

            Solution s = new Solution()
            {
                Bits = b,
            };

            HIFF deceptiveTrap = new HIFF();

            Assert.Equal(32.0, deceptiveTrap.Fitness(s));

            b.Set(2);

            b.Set(4);
            b.Set(5);
            b.Set(6);
            b.Set(7);

            Assert.Equal(18.0, deceptiveTrap.Fitness(s));

            b.Set(2, false);

            Assert.Equal(24.0, deceptiveTrap.Fitness(s));

            b = new BitList(16);

            s.Bits = b;

            Assert.Equal(80, deceptiveTrap.Fitness(s));

            b = new BitList(8);
            b.Set(0);
            b.Set(2);
            b.Set(6);
            b.Set(7);
            s.Bits = b;

            Assert.Equal(12, deceptiveTrap.Fitness(s));
        }


        [Fact]
        public void TestMaxSat()
        {
            BitList b = new BitList(10);

            Solution s = new Solution()
            {
                Bits = b,
            };

            MaxSat maxSat = new MaxSat();

            Assert.Equal(9, maxSat.Fitness(s));

            b.Set(1);

            Assert.Equal(10, maxSat.Fitness(s));

            b.Set(2);

            Assert.Equal(11, maxSat.Fitness(s));

            b.Set(4);

            Assert.Equal(11, maxSat.Fitness(s));

            b.Set(6,true);

            Assert.Equal(11, maxSat.Fitness(s));

            b.Set(4, false);

            Assert.Equal(10, maxSat.Fitness(s));

            b.Set(7);
            Assert.Equal(10, maxSat.Fitness(s));

            b.Set(8);
            Assert.Equal(10, maxSat.Fitness(s));

            b.Set(9);
            Assert.Equal(11, maxSat.Fitness(s));

            b.Flip(6);
            Assert.Equal(12, maxSat.Fitness(s));


        }
    }

}
