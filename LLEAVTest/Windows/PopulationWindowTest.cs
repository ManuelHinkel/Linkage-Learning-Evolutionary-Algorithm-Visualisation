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

namespace LLEAVTest.Windows
{
 
    public class PopulationWindowTest: TestClass
    {
        private readonly ITestOutputHelper _out;
        public PopulationWindowTest(ITestOutputHelper testOutputHelper)
        {
            _out = testOutputHelper;

            tests = [
                TestOpenPopulationWindow,
                TestTreeChange
            ];
            
        }

   
        public async void TestOpenPopulationWindow()
        {
            Thread.Sleep(1000);

            Helpers.ChangeAnimationModus(1);

            Helpers.CreateAlgorithmRun(16, 2, 2, 0, "10", 0, 0, 0);

            await Task.Delay(500);

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


        public async void TestTreeChange()
        {
            Thread.Sleep(1000);

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

            Thread.Sleep(500);

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
