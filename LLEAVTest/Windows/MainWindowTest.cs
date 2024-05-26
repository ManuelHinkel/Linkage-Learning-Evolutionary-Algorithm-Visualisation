
using Avalonia.Controls;
using LLEAV.Views.Windows;
using LLEAV.ViewModels;
using Avalonia.Interactivity;
using System.Xml.Linq;
using Avalonia.Threading;
using Xunit.Extensions.Ordering;
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

            await Task.Delay(500);

            Helpers.PressButton("Cancel", newAlgorithmWindow);

            Helpers.WaitFor(() => newAlgorithmWindow.IsVisible);
        }

        [Fact, Order(2)]
        public async void TestCreateAlgorithmRun()
        {
            await Task.Delay(100);
            var w = GlobalManager.Instance.MainWindow;
            var b = w.FindControl<Button>("PlayButton");

            Helpers.WaitFor(() => b.IsEnabled);
            Helpers.CreateAlgorithmRun(20, 1, 2, 0, "10", 0);

            Helpers.WaitFor(() => b.IsEnabled);

            var i = Helpers.Find<IterationDetailWindow>();

            Helpers.WaitFor(() => i.IsVisible);

            Assert.True(((IterationDetailWindowViewModel)i.DataContext).ContentViewModel is MIPIterationViewModel);
        }

        [Fact, Order(3)]
        public async void TestCreateAlgorithmRun2()
        {
            await Task.Delay(100);

            var w = GlobalManager.Instance.MainWindow;
            var b = w.FindControl<Button>("PlayButton");

            Helpers.WaitFor(() => b.IsEnabled);

            Helpers.CreateAlgorithmRun(20, 1, 2, 0, "10", 2);

            Helpers.WaitFor(() => b.IsEnabled);

            var i = Helpers.Find<IterationDetailWindow>();

            Helpers.WaitFor(() => i.IsVisible);

            Assert.True(((IterationDetailWindowViewModel)i.DataContext).ContentViewModel is ROMIterationViewModel);
        }

        [Fact, Order(4)]
        public async void TestPopulationDepiction()
        {
            await Task.Delay(1000);

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
                Assert.Fail("Should only show a Population Block");
            }

            /*HIFF h = new HIFF();
            Solution s = new Solution();


            BitList b = new BitList(8);

            b.Set(2);
            b.Set(3);

            b.Set(4);
            b.Set(5);
            b.Set(6);
            b.Set(7);

            s.Bits = b;
            _out.WriteLine(h.Fitness(s).ToString());*/
        }
    }
}