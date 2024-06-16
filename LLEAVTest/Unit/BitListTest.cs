using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LLEAVTest.Unit
{
    public class BitListTest
    {
        [Fact]
        public void TestAnd()
        {
            BitList b1 = new BitList(10);
            b1.Set(0);
            b1.Set(1);
            b1.Set(9);
            b1.Set(8);

            BitList b2 = new BitList(10);
            b2.Set(0);
            b2.Set(9);

            Assert.Equal(b2,b1 & b2);

        }

        [Fact]
        public void TestOr()
        {
            BitList b1 = new BitList(10);
            b1.Set(0);
            b1.Set(1);
            b1.Set(9);
            b1.Set(8);

            BitList b2 = new BitList(10);
            b2.Set(1);
            b2.Set(7);

            BitList b3 = new BitList(10);
            b3.Set(0);
            b3.Set(1);
            b3.Set(9);
            b3.Set(8);
            b3.Set(7);

            Assert.Equal(b3, b1 | b2);

        }

        [Fact]
        public void TestFlip()
        {
            BitList b1 = new BitList(10);
            b1.Set(0);
            b1.Set(1);
            b1.Set(9);
            b1.Set(8);

            b1.Flip(2);

            Assert.Equal("1110 0000 11", b1.ToString());

            b1.Flip(2);

            Assert.Equal("1100 0000 11", b1.ToString());
        }

        [Fact]
        public void TestLongList()
        {
            BitList b1 = new BitList(128);
            b1.Set(64);

            Assert.Equal("0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 1000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000", b1.ToString());
            b1.Set(127);
            Assert.True(b1.Get(127));
            b1.Set(63);
            Assert.Equal("0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0001 1000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0001", b1.ToString());
            b1.Flip(127);
            Assert.Equal("0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0001 1000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000", b1.ToString());
        }
    }
}
