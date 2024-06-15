using Avalonia.Controls;
using LLEAV.ViewModels.Controls.PopulationDepictions;
using LLEAV.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Avalonia.LogicalTree;
using LLEAV.Views.Windows;
using LLEAV.Models.Tree;
using LLEAV.ViewModels.Windows;
using LLEAV.Models.Algorithms.ROM;
using LLEAV.Models.FitnessFunction;
using LLEAV.Models.FOSFunction;
using LLEAV.Models.TerminationCriteria;
using LLEAV.Models.Algorithms.MIP;
using LLEAV.Models.LocalSearchFunction;
using LLEAV.Models.GrowthFunction;
using Avalonia.Threading;
using LLEAV;

namespace LLEAVTest.Windows
{
 
    public class PopulationWindowTest: TestClass
    {
        public PopulationWindowTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            tests = [
                TestOpenPopulationWindow,
                TestTreeChange
         ];
        }

        public async void TestOpenPopulationWindow()
        {
            var w = GlobalManager.Instance.MainWindow;
            Dispatcher.UIThread.Invoke(() =>
            {
                Helpers.ChangeAnimationModus(1);
            });

            Thread.Sleep(100);

            Dispatcher.UIThread.Invoke(() =>
            {
                Expect.Equal(1, Helpers.GetAnimationModus(), "Animation modus is wrong.");
                Helpers.CreateAlgorithmRun(16, typeof(HIFF), typeof(LinkageTreeFOS), typeof(IterationTermination), "10", typeof(MIP), typeof(HillClimber), typeof(ConstantGrowth));
            });

            Thread.Sleep(1000);

            Dispatcher.UIThread.Invoke(() =>
            {
                Helpers.CloseWindow<IterationDetailWindow>();
            });

            Thread.Sleep(1000);

            var app = AvaloniaApp.GetApp();
            Dispatcher.UIThread.Invoke(() =>
            {

                Expect.False(app.Windows.OfType<IterationDetailWindow>().Count() > 0, "Iteration Details Window was not closed");
                var p = Helpers.Find<PopulationWindow>();

                Expect.True(p.IsEffectivelyVisible, "Population window not open.");
            });

          

        }


        public async void TestTreeChange()
        {
            var w = GlobalManager.Instance.MainWindow;
            Dispatcher.UIThread.Invoke(() =>
            {
                var p = Helpers.Find<PopulationWindow>();
                bool nodeMarked = false;

                foreach (var c in p.GetLogicalDescendants().OfType<Control>())
                {

                    if (c.DataContext != null && c.DataContext.GetType().Equals(typeof(Node)))
                    {
                        Node n = c.DataContext as Node;

                        if (n.IsNewNode)
                        {
                            nodeMarked = true;
                        }

                    }

                }
                Expect.False(nodeMarked, "No node should be marked.");
                Helpers.NextIteration();
            });


            Thread.Sleep(1000);

            Dispatcher.UIThread.Invoke(() =>
            {
                var p = Helpers.Find<PopulationWindow>();
                bool nodeMarked = false;
                foreach (var c in p.GetLogicalDescendants().OfType<Control>())
                {
                    if (c.DataContext != null && c.DataContext.GetType().Equals(typeof(Node)))
                    {
                        Node n = c.DataContext as Node;

                        if (n.IsNewNode)
                        {
                            nodeMarked = true;
                        }

                    }

                }
                Expect.True(nodeMarked, "A node should be marked.");
            });
        }

    }
}
