using HarfBuzzSharp;
using LLEAV.Models;
using LLEAV.Models.FitnessFunction;
using LLEAV.Util;
using Xunit.Abstractions;

namespace LLEAVTest.Unit
{
    public class FitnessFunctionTest
    {

        private readonly ITestOutputHelper _out;
        public FitnessFunctionTest(ITestOutputHelper testOutputHelper)
        {
            _out = testOutputHelper;
        }

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

            Assert.Equal(5.0, oneMax.Fitness(s));

            b.Set(2);

            Assert.Equal(6.0, oneMax.Fitness(s));

            b.Set(0, false);

            Assert.Equal(5, oneMax.Fitness(s));
        }

        [Fact]
        public void TestLeadingOnes()
        {
            BitList b = new BitList(10);
            b.Set(0);
            b.Set(1);

            Solution s = new Solution()
            {
                Bits = b,
            };

            LeadingOnes leadingOnes = new LeadingOnes();

            Assert.Equal(2.0, leadingOnes.Fitness(s));

            b.Set(3);
            b.Set(4);
            b.Set(5);

            Assert.Equal(2.0, leadingOnes.Fitness(s));

            b.Set(2);

            Assert.Equal(6.0, leadingOnes.Fitness(s));

            b.Set(0, false);

            Assert.Equal(0, leadingOnes.Fitness(s));
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
            DeceptiveTrap copy = new DeceptiveTrap();

            // k is 7 when creating, same as original
            copy.CreateArgumentFromString("1");

            var bytes = deceptiveTrap.ConvertArgumentToBytes();
            copy.CreateArgumentFromBytes(bytes);

            Assert.Equal(12.0, deceptiveTrap.Fitness(s));
            Assert.Equal(12.0, copy.Fitness(s));


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
            Assert.Equal(0, copy.Fitness(s));


            b.Set(6);
            b.Set(13);

            Assert.Equal(14.0, deceptiveTrap.Fitness(s));
            Assert.Equal(14.0, copy.Fitness(s));


        }

        [Fact]
        public void TestHIFF()
        {
            BitList b = new BitList(8);

            Solution s = new Solution()
            {
                Bits = b,
            };

            HIFF hiff = new HIFF();

            Assert.Equal(32.0, hiff.Fitness(s));

            b.Set(2);

            b.Set(4);
            b.Set(5);
            b.Set(6);
            b.Set(7);

            Assert.Equal(18.0, hiff.Fitness(s));

            b.Set(2, false);

            Assert.Equal(24.0, hiff.Fitness(s));

            b = new BitList(16);

            s.Bits = b;

            Assert.Equal(80, hiff.Fitness(s));

            b = new BitList(8);
            b.Set(0);
            b.Set(2);
            b.Set(6);
            b.Set(7);
            s.Bits = b;

            Assert.Equal(12, hiff.Fitness(s));
        }

        [Fact]
        public void TestNKLandscape()
        {
            BitList b = new BitList(8);

            Solution s = new Solution()
            {
                Bits = b,
            };

            NKLandscape landscape = new NKLandscape();

            Assert.Equal(32.0, landscape.Fitness(s));

            Assert.Equal(40.0, landscape.Fitness(new Solution()
            {
                Bits = !b,
            }));

            b.Set(0);
            Assert.Equal(29.0, landscape.Fitness(s));

            b.Set(4);
            Assert.Equal(26.0, landscape.Fitness(s));

            b.Set(1);
            Assert.Equal(17.0, landscape.Fitness(s));

        }

        [Fact]
        public void TestNKLandscapeRandom()
        {
            BitList b = new BitList(8);

            Solution s = new Solution()
            {
                Bits = b,
            };

            NKLandscapeRandom landscape = new NKLandscapeRandom();
            NKLandscapeRandom copy = new NKLandscapeRandom();
            copy.CreateArgumentFromBytes(landscape.ConvertArgumentToBytes());

            Assert.Equal(landscape.Fitness(s), copy.Fitness(s));


            b.Set(0);
            Assert.Equal(landscape.Fitness(s), copy.Fitness(s));

            b.Set(4);
            Assert.Equal(landscape.Fitness(s), copy.Fitness(s));

            b.Set(1);
            Assert.Equal(landscape.Fitness(s), copy.Fitness(s));

        }

        [Fact]
        public void TestIsingRing()
        {
            BitList b = new BitList(8);

            Solution s = new Solution()
            {
                Bits = b,
            };

            IsingRing ring = new IsingRing();

            Assert.Equal(8.0, ring.Fitness(s));

            b.Set(0);

            Assert.Equal(6.0, ring.Fitness(s));

            b.Set(2);

            Assert.Equal(4.0, ring.Fitness(s));

            b.Set(4);

            Assert.Equal(2.0, ring.Fitness(s));

            b.Set(6);

            Assert.Equal(0.0, ring.Fitness(s));

            b.Set(1);

            Assert.Equal(2.0, ring.Fitness(s));

            b.Set(3);

            Assert.Equal(4.0, ring.Fitness(s));

            b.Set(5);

            Assert.Equal(6.0, ring.Fitness(s));
            
            b.Set(7);

            Assert.Equal(8.0, ring.Fitness(s));

        }

        [Fact]
        public void TestIsingLattice()
        {
            BitList b = new BitList(9);

            Solution s = new Solution()
            {
                Bits = b,
            };

            IsingLattice lattice = new IsingLattice();
            IsingLattice copy = new IsingLattice();
            lattice.CreateArgumentFromString("3 3");
            var bytes = lattice.ConvertArgumentToBytes();
            copy.CreateArgumentFromBytes(bytes);

            Assert.Equal(18.0, lattice.Fitness(s));
            Assert.Equal(18.0, copy.Fitness(s));
            Assert.Equal(18.0, lattice.Fitness(new Solution()
            {
                Bits = !b,
            }));

            b.Set(0); //100 000 000

            Assert.Equal(14.0, lattice.Fitness(s));
            Assert.Equal(14.0, copy.Fitness(s));

            b.Set(5);//100 001 000

            Assert.Equal(10.0, lattice.Fitness(s));
            Assert.Equal(10.0, copy.Fitness(s));

            b.Set(3);//100 101 000

            Assert.Equal(10.0, lattice.Fitness(s));
            Assert.Equal(10.0, copy.Fitness(s));


            b.Set(4);//100 111 000

            Assert.Equal(10.0, lattice.Fitness(s));
            Assert.Equal(10.0, copy.Fitness(s));


            b.Set(8);//100 111 010

            Assert.Equal(8.0, lattice.Fitness(s));
            Assert.Equal(8.0, copy.Fitness(s));


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
            MaxSat maxSatcopy = new MaxSat();

            Assert.True(maxSat.CreateArgumentFromString("(0 | 1) & (!0 | !1) & (2) & (!3) & (4 | 5 | !6) & (7 | 8 | 9) & (7 | 8 | !9) & (7 | !8 | 9) & (7 | !8 | !9) & (!7 | 8 | 9) & (!7 | 8 | !9) & (!7 | !8 | 9)"));

            var bytes = maxSat.ConvertArgumentToBytes();
            maxSatcopy.CreateArgumentFromBytes(bytes);

            Assert.Equal(9, maxSat.Fitness(s));
            Assert.Equal(9, maxSatcopy.Fitness(s));

            b.Set(1);

            Assert.Equal(10, maxSat.Fitness(s));
            Assert.Equal(10, maxSatcopy.Fitness(s));

            b.Set(2);

            Assert.Equal(11, maxSat.Fitness(s));
            Assert.Equal(11, maxSatcopy.Fitness(s));

            b.Set(4);

            Assert.Equal(11, maxSat.Fitness(s));
            Assert.Equal(11, maxSatcopy.Fitness(s));

            b.Set(6,true);

            Assert.Equal(11, maxSat.Fitness(s));
            Assert.Equal(11, maxSatcopy.Fitness(s));

            b.Set(4, false);

            Assert.Equal(10, maxSat.Fitness(s));
            Assert.Equal(10, maxSatcopy.Fitness(s));

            b.Set(7);
            Assert.Equal(10, maxSat.Fitness(s));
            Assert.Equal(10, maxSatcopy.Fitness(s));

            b.Set(8);
            Assert.Equal(10, maxSat.Fitness(s));
            Assert.Equal(10, maxSatcopy.Fitness(s));

            b.Set(9);
            Assert.Equal(11, maxSat.Fitness(s));
            Assert.Equal(11, maxSatcopy.Fitness(s));

            b.Flip(6);
            Assert.Equal(12, maxSat.Fitness(s));
            Assert.Equal(12, maxSatcopy.Fitness(s));


        }
    }
}
