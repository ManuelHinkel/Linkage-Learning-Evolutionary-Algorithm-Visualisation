
using Avalonia.Controls;
using LLEAV.Views.Windows;
using LLEAV.ViewModels;
using Avalonia.Interactivity;
using System.Xml.Linq;
using Avalonia.Threading;
using LLEAV.ViewModels.Controls.IterationDepictions;
using System.Diagnostics;
using Xunit.Abstractions;
using LLEAV.ViewModels.Windows;
using Avalonia.LogicalTree;
using LLEAV.ViewModels.Controls.PopulationDepictions;
using LLEAV.Views.Controls.PopulationDepictions;
using LLEAV.Models.FitnessFunction;
using LLEAV.Util;
using LLEAV.Models;
using System.Collections;
using Xunit.Extensions.Ordering;

namespace LLEAVTest
{
    [Order(1)]
    public class MainWindowTest
    {
        private readonly ITestOutputHelper _out;
        public MainWindowTest(ITestOutputHelper testOutputHelper)
        {
            _out = testOutputHelper;
        }


        [Fact, Order(1)]
        public async void TestNewAlgorithmCancelWindow()
        {
            var app = AvaloniaApp.GetApp();

            var w = GlobalManager.Instance.MainWindow;
            var b = w.FindControl<Button>("PlayButton");

            Assert.False(b.IsEnabled);

            var f = w.FindControl<MenuItem>("File");

            f.Open();

            var n = w.FindControl<MenuItem>("NewFile");

            Helpers.WaitFor(() => n.IsVisible);

            var clickEvent = new RoutedEventArgs(MenuItem.ClickEvent);
            n.RaiseEvent(clickEvent);

            var newAlgorithmWindow = app.Windows.OfType<NewAlgorithmWindow>().Single();

            Helpers.WaitFor(() => newAlgorithmWindow.IsVisible);

            var g = newAlgorithmWindow.FindControl<ComboBox>("GrowthFunction");

            Assert.True(g.IsVisible);

            var p = newAlgorithmWindow.FindControl<NumericUpDown>("PopulationSize");
            Assert.False(p.IsVisible);

            Thread.Sleep(500);

            Helpers.PressButton("Cancel", newAlgorithmWindow);

            Helpers.WaitFor(() => newAlgorithmWindow.IsVisible);
        }

        [Fact, Order(2)]
        public async void TestCreateAlgorithmRun()
        {
            Thread.Sleep(100);
            var w = GlobalManager.Instance.MainWindow;
            var b = w.FindControl<Button>("PlayButton");

            Helpers.WaitFor(() => b.IsEnabled);
            Helpers.CreateAlgorithmRun(20, 1, 2, 0, "10", 0);

            Helpers.WaitFor(() => b.IsEnabled);

            var i = Helpers.Find<IterationDetailWindow>();

            Helpers.WaitFor(() => i.IsVisible);

            Assert.Equal(typeof(MIPIterationViewModel), i.Content.GetType());
        }
        [Fact, Order(3)]
        public async void TestCreateAlgorithmRun2()
        {
            Thread.Sleep(100);

            var w = GlobalManager.Instance.MainWindow;
            var b = w.FindControl<Button>("PlayButton");

            Helpers.WaitFor(() => b.IsEnabled);

            Helpers.CreateAlgorithmRun(20, 1, 2, 0, "10", 2);

            Helpers.WaitFor(() => b.IsEnabled);

            var i = Helpers.Find<IterationDetailWindow>();

            Helpers.WaitFor(() => i.IsVisible);

            Assert.Equal(typeof(ROMIterationViewModel), i.Content.GetType());
        }
        [Fact, Order(4)]
        public async void TestPopulationDepiction()
        {
            Thread.Sleep(1000);

            var w = GlobalManager.Instance.MainWindow;

            bool foundPopulationBlock = false;
            foreach (var c in w.GetLogicalDescendants().OfType<Control>())
            {
                var control = (Control)c;
                if (control.DataContext != null && control.DataContext.GetType().IsSubclassOf(typeof(PopulationContainerViewModelBase)))
                {
                    if (control.DataContext.GetType().Equals(typeof(PopulationBlock)))
                    {
                        foundPopulationBlock = true;
                    } else
                    {
                        Assert.Fail("Should only show PopulationBlocks");
                    }
                }
            }
            if (!foundPopulationBlock)
            {
                Assert.Fail("Should show a Population Block");
            }

        }
        [Fact, Order(5)]
        public async void TestNextIteration()
        {
            _out.WriteLine("Next Iteration");
            Thread.Sleep(2000);

            Helpers.ChangeAnimationModus(1);

            Helpers.NextIteration();

            Helpers.CloseAllExceptMain();

            var app = AvaloniaApp.GetApp();
            Helpers.WaitFor(() => app.Windows.Count == 1, 500);
        }
        [Fact, Order(6)]
        public async void TestOtherAlgorithms()
        {
            _out.WriteLine("Other Algorithms");

            Thread.Sleep(1000);

            Helpers.ChangeAnimationModus(0);

            Helpers.WaitFor(() => Helpers.GetAnimationModus() == 0);

            Helpers.CreateAlgorithmRun(14, 2, 1, 0, "10", 1, 0);

            var i = Helpers.Find<IterationDetailWindow>();

            Helpers.WaitFor(() => i.IsVisible);
            Assert.Equal(typeof(MIPIterationViewModel),i.Content.GetType());

            Thread.Sleep(1000);

            Helpers.CreateAlgorithmRun(16, 3, 0, 0, "10", 3, populationSize: 10);

            Thread.Sleep(1000);

            i = Helpers.Find<IterationDetailWindow>();

            Assert.Equal(typeof(GOMIterationViewModel), i.Content.GetType());
        }

      
    }
}