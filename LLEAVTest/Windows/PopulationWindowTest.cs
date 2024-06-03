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
using Xunit.Extensions.Ordering;

namespace LLEAVTest.Windows
{
    [Order(2)]
    public class PopulationWindowTest
    {
        private readonly ITestOutputHelper _out;
        public PopulationWindowTest(ITestOutputHelper testOutputHelper)
        {
            _out = testOutputHelper;
        }

        [Fact, Order(1)]
        public async void TestOpenPopulationWindow()
        {
            _out.WriteLine("Open Population");
            Thread.Sleep(1000);

            Helpers.ChangeAnimationModus(1);

            Helpers.CreateAlgorithmRun(14, 1, 2, 0, "10", 0, 0, 0);

            Helpers.CloseWindow<IterationDetailWindow>();

            var w = GlobalManager.Instance.MainWindow;

            bool foundPopulationBlock = false;
            foreach (var c in w.GetLogicalDescendants().OfType<Control>())
            {
                var control = c;
                if (control.DataContext != null && control.DataContext.GetType().IsSubclassOf(typeof(PopulationContainerViewModelBase)))
                {
                    if (control.DataContext.GetType().Equals(typeof(PopulationBlock)))
                    {
                        if (control.GetType().Equals(typeof(Button)))
                        {
                            Helpers.PressButton((Button)control);
                        }
                        foundPopulationBlock = true;
                    }
                    else
                    {
                        Assert.Fail("Should only show PopulationBlocks");
                    }
                }
            }
            if (!foundPopulationBlock)
            {
                Assert.Fail("Should show a Population Block");
            }

            var p = Helpers.Find<PopulationWindow>();

            Helpers.WaitFor(() => p.IsVisible);
        }

        [Fact, Order(2)]
        public async void TestTreeChange()
        {
            await Task.Delay(5000);

            var p = Helpers.Find<PopulationWindow>();

            Helpers.WaitFor(() => p.IsVisible);

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
            Assert.False(nodeMarked);

            Helpers.NextIteration();

            await Task.Delay(1000);


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
            Assert.True(nodeMarked);

        }

    }
}
