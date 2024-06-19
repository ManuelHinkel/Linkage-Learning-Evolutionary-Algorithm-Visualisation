using Avalonia.Animation;
using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAVTest.Unit
{
    public class Utiltest
    {
        [Fact]
        public void TestFromHSV()
        {
            Assert.Equal("#965D51", HSVConverter.FromHSV(10, 0.46, 0.59));
            Assert.Equal("#2E3314", HSVConverter.FromHSV(70, 0.6, 0.2));
            Assert.Equal("#47965C", HSVConverter.FromHSV(136, 0.53, 0.59));
            Assert.Equal("#27292B", HSVConverter.FromHSV(210, 0.09, 0.17));
            Assert.Equal("#9484BF", HSVConverter.FromHSV(256, 0.31, 0.75));
            Assert.Equal("#D40004", HSVConverter.FromHSV(359, 1, 0.83));
        }
    }
}
