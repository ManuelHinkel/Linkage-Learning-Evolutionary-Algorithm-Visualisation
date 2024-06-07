
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

namespace LLEAVTest.Windows
{
    public class MainWindowTest: TestClass
    {
        private readonly ITestOutputHelper _out;
        public MainWindowTest(ITestOutputHelper testOutputHelper)
        {
            _out = testOutputHelper;

            tests = [
                TestNewAlgorithmCancelWindow,
                TestCreateAlgorithmRun,
                TestCreateAlgorithmRun2,
                TestPopulationDepiction,
                TestNextIteration,
                TestOtherAlgorithms
            ];
        }


        public async void TestNewAlgorithmCancelWindow()
        {
            var app = AvaloniaApp.GetApp();

            var w = GlobalManager.Instance.MainWindow;
            var b = w.FindControl<Button>("PlayButton");

            Helpers.WaitFor(() => !b.IsEnabled);

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

        public async void TestCreateAlgorithmRun()
        {
            Thread.Sleep(100);
            var w = GlobalManager.Instance.MainWindow;
            var b = w.FindControl<Button>("PlayButton");

            Helpers.WaitFor(() => b.IsEnabled);
            Helpers.CreateAlgorithmRun(20, 0, 2, 0, "10", 0);

            Helpers.WaitFor(() => b.IsEnabled);

            var i = Helpers.Find<IterationDetailWindow>();

            Helpers.WaitFor(() => i.IsVisible);

            Assert.Equal(typeof(MIPIterationViewModel), i.Content.GetType());
        }
 
        public async void TestCreateAlgorithmRun2()
        {
            Thread.Sleep(100);

            var w = GlobalManager.Instance.MainWindow;
            var b = w.FindControl<Button>("PlayButton");

            Helpers.WaitFor(() => b.IsEnabled);

            Helpers.CreateAlgorithmRun(20, 0, 2, 0, "10", 2);

            Helpers.WaitFor(() => b.IsEnabled);

            var i = Helpers.Find<IterationDetailWindow>();

            Helpers.WaitFor(() => i.IsVisible);

            Assert.Equal(typeof(ROMIterationViewModel), i.Content.GetType());
        }

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
 
        public async void TestNextIteration()
        {
            Thread.Sleep(2000);

            Helpers.ChangeAnimationModus(1);

            Helpers.NextIteration();

            Helpers.CloseAllExceptMain();

            var app = AvaloniaApp.GetApp();
            Helpers.WaitFor(() => app.Windows.Count == 1, 500);
        }
 
        public async void TestOtherAlgorithms()
        {
            Thread.Sleep(1000);

            Helpers.ChangeAnimationModus(0);

            Helpers.WaitFor(() => Helpers.GetAnimationModus() == 0);

            Helpers.CreateAlgorithmRun(14, 1, 1, 0, "10", 1, 0);

            var i = Helpers.Find<IterationDetailWindow>();

            Helpers.WaitFor(() => i.IsVisible);
            Assert.Equal(typeof(MIPIterationViewModel),i.Content.GetType());

            Thread.Sleep(1000);

            Helpers.CreateAlgorithmRun(16, 2, 0, 0, "10", 3, populationSize: 10);

            Thread.Sleep(1000);

            i = Helpers.Find<IterationDetailWindow>();

            Assert.Equal(typeof(GOMIterationViewModel), i.Content.GetType());
        }

      
    }
}