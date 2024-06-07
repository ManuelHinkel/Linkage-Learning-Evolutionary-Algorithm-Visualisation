using LLEAV.Models;
using LLEAV.Models.Algorithms.MIP.StateChange;
using LLEAV.Models.Algorithms.ROM.StateChange;
using LLEAV.ViewModels.Controls;
using LLEAV.ViewModels.Controls.IterationDepictions;
using LLEAV.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace LLEAVTest.Windows
{
    public class IterationDetailsWindowTest: TestClass
    {
        private readonly ITestOutputHelper _out;
        public IterationDetailsWindowTest(ITestOutputHelper testOutputHelper)
        {
            _out = testOutputHelper;
            tests = [
                TestMIP,
                TestROM,
                TestGOM,
                TestGlobalStep
            ];
        }

        public void TestMIP()
        {
            Thread.Sleep(500);
            Helpers.ChangeAnimationModus(0);
            Helpers.CreateAlgorithmRun(16, 2, 0, 0, "10", 0, 0, 1);

            var i = Helpers.Find<IterationDetailWindow>();

            Helpers.WaitFor(() => i.IsVisible);

            Assert.Equal(typeof(MIPIterationViewModel), i.Content.GetType());

            var content = i.Content as MIPIterationViewModel;

            content.CurrentStateChange = content.MaxStateChange;

            
            var message = content.MessageBox.Messages.First(m => m.Content.StartsWith("Merged the parents resulting in:"));
            Assert.Equal(message.Content.Split("\n")[1], content.Merged.Solution.Bits.ToString());

            Assert.Equal("Changed the population currently viewed.", content.MessageBox.Messages.Last().Content);
            Assert.Equal("Termination criteria was not met.", content.MessageBox.Messages[0].Content);

        }

        public void TestROM()
        {
            Thread.Sleep(500);
            Helpers.CreateAlgorithmRun(16, 2, 0, 0, "0", 2, populationSize: 30);

            var i = Helpers.Find<IterationDetailWindow>();

            Helpers.WaitFor(() => i.IsVisible);

            Assert.Equal(typeof(ROMIterationViewModel), i.Content.GetType());

            var content = i.Content as ROMIterationViewModel;

            content.CurrentStateChange = content.MaxStateChange;
            Thread.Sleep(500);

            Assert.Equal("Changed the population currently viewed.", content.MessageBox.Messages.Last().Content);
            Assert.Equal("Termination criteria was met.", content.MessageBox.Messages[0].Content);

            var message = content.MessageBox.Messages.First(m => m.Content.Contains("and:"));

            Assert.True(message.Content.Contains(content.CurrentSolution1.Solution.Bits.ToString()));
            Assert.True(message.Content.Contains(content.CurrentSolution2.Solution.Bits.ToString()));
        }

        public void TestGOM()
        {
            Thread.Sleep(500);
            Helpers.CreateAlgorithmRun(16, 2, 0, 1, "16.0", 3, populationSize: 30);

            var i = Helpers.Find<IterationDetailWindow>();

            Helpers.WaitFor(() => i.IsVisible);

            Assert.Equal(typeof(GOMIterationViewModel), i.Content.GetType());

            var content = i.Content as GOMIterationViewModel;

            content.CurrentStateChange = content.MaxStateChange;
            Thread.Sleep(500);

            Assert.Equal("Changed the population currently viewed.", content.MessageBox.Messages.Last().Content);
            Assert.True(content.MessageBox.Messages[content.MessageBox.Messages.Count()-2].Content.Contains("Changed active solution to:"));
            Assert.Equal("Termination criteria was met.", content.MessageBox.Messages[0].Content);


            var message = content.MessageBox.Messages.First(m => m.Content.StartsWith("Changed the donor to:"));

            Assert.True(message.Content.Contains(content.CurrentDonor.Solution.Bits.ToString()));
        }

        public void TestGlobalStep()
        {
            Thread.Sleep(500);

            var i = Helpers.Find<IterationDetailWindow>();

            var content = i.Content as GOMIterationViewModel;

            Helpers.WaitFor(() => content.ForwardButtonEnabled);

            content.GlobalStepForward();

            Helpers.WaitFor(() => !content.ForwardButtonEnabled);
            Helpers.WaitFor(() => content.BackwardButtonEnabled);

            content.GlobalStepBackward();

            Helpers.WaitFor(() => !content.ForwardButtonEnabled);
            Helpers.WaitFor(() => !content.BackwardButtonEnabled);



        }
    }
}
