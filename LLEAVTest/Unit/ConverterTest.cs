using LLEAV.Converter;
using LLEAV.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAVTest.Unit
{
    public class ConverterTest
    {
        [Fact]
        public void TestAnimationModusConverter()
        {
            AnimationModusConverter converter = new AnimationModusConverter();
            Assert.True((bool)converter.Convert(AnimationModus.FOS, typeof(AnimationModus), "FOS", null));
            Assert.False((bool)converter.Convert(AnimationModus.NONE, typeof(AnimationModus), "FOS", null));
            Assert.False((bool)converter.Convert(AnimationModus.NONE, typeof(AnimationModus), "FULL", null));
            Assert.True((bool)converter.Convert(AnimationModus.FULL, typeof(AnimationModus), "FULL", null));
            Assert.ThrowsAny<Exception>(() => converter.Convert(AnimationModus.FULL, typeof(AnimationModus), "", null));
        }

        [Fact]
        public void TestIntEqualsConverter()
        {
            IntEqualsConverter converter = new IntEqualsConverter();
            Assert.True((bool)converter.Convert(10, typeof(int), "10", null));
            Assert.ThrowsAny<Exception>(() => converter.Convert(AnimationModus.FULL, typeof(AnimationModus), "", null));
        }

        [Fact]
        public void TestPopulationSizeToBlockWidthConverter()
        {
            PopulationSizeToBlockWidthConverter converter = new PopulationSizeToBlockWidthConverter();
            Assert.Equal(500, converter.Convert(1024, typeof(int), null, null));
            Assert.Equal(300, converter.Convert(0, typeof(int), null, null));
            Assert.Equal(320, converter.Convert(2, typeof(int), null, null));
            Assert.ThrowsAny<Exception>(() => converter.Convert(AnimationModus.FULL, typeof(AnimationModus), "", null));
        }

    }
}
