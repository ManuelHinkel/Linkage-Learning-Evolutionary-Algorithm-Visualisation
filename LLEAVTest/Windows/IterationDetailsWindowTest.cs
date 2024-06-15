﻿using Avalonia.Controls;
using Avalonia.Threading;
using LLEAV.Models;
using LLEAV.Models.Algorithms.GOM;
using LLEAV.Models.Algorithms.MIP;
using LLEAV.Models.Algorithms.MIP.StateChange;
using LLEAV.Models.Algorithms.ROM;
using LLEAV.Models.Algorithms.ROM.StateChange;
using LLEAV.Models.FitnessFunction;
using LLEAV.Models.FOSFunction;
using LLEAV.Models.GrowthFunction;
using LLEAV.Models.LocalSearchFunction;
using LLEAV.Models.TerminationCriteria;
using LLEAV.ViewModels;
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
        public IterationDetailsWindowTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            tests = [
                TestROM,
                TestGOM,
                TestMIP,
                TestGlobalStep
          ];
        }

        public void TestROM()
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                Helpers.ChangeAnimationModus(0);
            });

            Thread.Sleep(100);

            Dispatcher.UIThread.Invoke(() =>
            {
                Expect.Equal(0, Helpers.GetAnimationModus(), "Animation modus is wrong.");
                Helpers.CreateAlgorithmRun(16, typeof(HIFF), typeof(UnivariateFOS), typeof(IterationTermination), "0", typeof(ROMEA), populationSize: 30);
            });


            Thread.Sleep(1000);

            Dispatcher.UIThread.Invoke(() =>
            {
                var i = Helpers.Find<IterationDetailWindow>();
                Expect.True(i.IsEffectivelyVisible, "Iteration details not visible.");
                Expect.Equal(typeof(ROMIterationViewModel), i.Content.GetType(), "Iteration Details has wrong type. (1)");
                var content = i.Content as ROMIterationViewModel;

                content.CurrentStateChange = content.MaxStateChange;

                Expect.Equal("Changed the population currently viewed.", content.MessageBox.Messages.Last().Content, "Wrong first message. (1)");
                Expect.Equal("Termination criteria was met.", content.MessageBox.Messages[0].Content, "Wrong last message. (1)");

                var message = content.MessageBox.Messages.First(m => m.Content.Contains("and:"));

                Expect.True(message.Content.Contains(content.CurrentSolution1.Solution.Bits.ToString()));
                Expect.True(message.Content.Contains(content.CurrentSolution2.Solution.Bits.ToString()));
            });
        }

        public void TestGOM()
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                Helpers.CreateAlgorithmRun(16, typeof(HIFF), typeof(UnivariateFOS), typeof(FitnessTermination), "16.0", typeof(GOMEA), populationSize: 30);
            });

            Thread.Sleep(1000);

            Dispatcher.UIThread.Invoke(() =>
            {
                var i = Helpers.Find<IterationDetailWindow>();
                Expect.True(i.IsEffectivelyVisible, "Iteration details not visible.");
                Expect.Equal(typeof(GOMIterationViewModel), i.Content.GetType(), "Iteration Details has wrong type. (2)");
                var content = i.Content as GOMIterationViewModel;

                content.CurrentStateChange = content.MaxStateChange;

                Expect.Equal("Changed the population currently viewed.", content.MessageBox.Messages.Last().Content, "Wrong first message. (2)");
                Expect.True(content.MessageBox.Messages[content.MessageBox.Messages.Count() - 2].Content.Contains("Changed active solution to:"));
                output.WriteLine(content.MessageBox.Messages[0].Content);
                Expect.Equal("Termination criteria was met.", content.MessageBox.Messages[0].Content, "Wrong last message. (2)");


                var message = content.MessageBox.Messages.First(m => m.Content.StartsWith("Changed the donor to:"));

                Expect.True(message.Content.Contains(content.CurrentDonor.Solution.Bits.ToString()), "Message does not contain curren donor.");
            });
        }

        public void TestMIP()
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                Expect.Equal(0, Helpers.GetAnimationModus(), "Animation modus is wrong.");
                Helpers.CreateAlgorithmRun(16, typeof(HIFF), typeof(UnivariateFOS), typeof(IterationTermination), "10", typeof(MIP), typeof(HillClimber), typeof(LinearGrowth));
            });


            Thread.Sleep(1000);

            Dispatcher.UIThread.Invoke(() =>
            {
                var i = Helpers.Find<IterationDetailWindow>();
                Expect.True(i.IsEffectivelyVisible, "Iteration details not visible.");
                Expect.Equal(typeof(MIPIterationViewModel), i.Content.GetType(), "Iteration Details has wrong type. (3)");
                var content = i.Content as MIPIterationViewModel;

                content.CurrentStateChange = content.MaxStateChange;


                var message = content.MessageBox.Messages.First(m => m.Content.StartsWith("Merged the parents resulting in:"));
                Expect.Equal(message.Content.Split("\n")[1], content.Merged.Solution.Bits.ToString(), "Message contained wrong content. (3)");

                Expect.Equal("Changed the population currently viewed.", content.MessageBox.Messages.Last().Content, "Wrong first message. (3)");
                Expect.Equal("Termination criteria was not met.", content.MessageBox.Messages[0].Content, "Wrong last message. (3)");
            });
        }

        public void TestGlobalStep()
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                var i = Helpers.Find<IterationDetailWindow>();
                Expect.True(i.IsEffectivelyVisible, "Iteration details not visible.");
                Expect.Equal(typeof(MIPIterationViewModel), i.Content.GetType(), "Iteration Details has wrong type. (4)");

                var content = i.Content as MIPIterationViewModel;
                Expect.True(content.ForwardButtonEnabled, "Forward not enabled");
                content.GlobalStepForward();
            });

            Thread.Sleep(1000);

            Dispatcher.UIThread.Invoke(() =>
            {
                var i = Helpers.Find<IterationDetailWindow>();
                var content = i.Content as MIPIterationViewModel;

                Expect.False(content.ForwardButtonEnabled, "Forward enabled");
                Expect.True(content.BackwardButtonEnabled, "Backward not enabled");
                content.GlobalStepBackward();
            });

            Thread.Sleep(1000);

            Dispatcher.UIThread.Invoke(() =>
            {
                var i = Helpers.Find<IterationDetailWindow>();
                var content = i.Content as MIPIterationViewModel;

                Expect.False(content.ForwardButtonEnabled, "Forward enabled");
                Expect.False(content.BackwardButtonEnabled, "Backward enabled");
                content.GlobalStepBackward();
            });
        }
    }
}
