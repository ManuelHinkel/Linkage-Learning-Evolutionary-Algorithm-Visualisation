
using Avalonia.Controls;
using LLEAV.Views.Windows;
using LLEAV.ViewModels;
using Avalonia.Interactivity;
using Avalonia.Threading;
using LLEAV.ViewModels.Controls.IterationDepictions;
using Xunit.Abstractions;
using Avalonia.LogicalTree;
using LLEAV.ViewModels.Controls.PopulationDepictions;
using LLEAV.Models.FitnessFunction;
using LLEAV.Models.FOSFunction;
using LLEAV.Models.TerminationCriteria;
using LLEAV.Models.Algorithms.ROM;
using LLEAV.Models.Algorithms.MIP;
using LLEAV.Models.LocalSearchFunction;
using LLEAV.Models.Algorithms.GOM;
using LLEAV.ViewModels.Controls;
using LLEAV.Views.Controls.PopulationDepictions;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;

namespace LLEAVTest.Windows
{
    public class MainWindowTest: TestClass
    {
        public MainWindowTest(ITestOutputHelper testOutputHelper): base(testOutputHelper)
        {
            tests = [
                TestNewAlgorithmCancelWindow,
                TestCreateAlgorithmRun1,
                TestCreateAlgorithmRun2,
                TestPopulationDepiction,
                TestNextIteration,
                TestOtherAlgorithms,
                TestPopulationDepictions
            ];
        }

        public void TestNewAlgorithmCancelWindow()
        {
            var app = AvaloniaApp.GetApp();

            var w = GlobalManager.Instance.MainWindow;
            Dispatcher.UIThread.Invoke(() =>
            {
                var b = w.FindControl<Button>("PlayButton");
                Expect.False(b.IsEffectivelyEnabled, "Play button was enabled.");
                var f = w.FindControl<MenuItem>("File");
                f.Open();
            });

            Thread.Sleep(500);

            Dispatcher.UIThread.Invoke(() =>
            {
                var n = w.FindControl<MenuItem>("NewFile");
                Expect.True(n.IsEffectivelyVisible, "New file option not visible.");

                var clickEvent = new RoutedEventArgs(MenuItem.ClickEvent);
                n.RaiseEvent(clickEvent);
            });

            Thread.Sleep(500);

            Dispatcher.UIThread.Invoke(() =>
            {
                var newAlgorithmWindow = app.Windows.OfType<NewAlgorithmWindow>().Single();
                Expect.True(newAlgorithmWindow.IsEffectivelyVisible, "New Algorithm Window was not visible.");
                
                var g = newAlgorithmWindow.FindControl<ComboBox>("GrowthFunction");
                Expect.True(g.IsEffectivelyVisible, "Growth Function option not visible.");

                var p = newAlgorithmWindow.FindControl<NumericUpDown>("PopulationSize");
                Expect.False(p.IsEffectivelyVisible, "Population size option visible.");
                Helpers.PressButton("Cancel", newAlgorithmWindow);
            });

            
            Thread.Sleep(500);

            Dispatcher.UIThread.Invoke(() =>
            {
                Expect.False(app.Windows.OfType<NewAlgorithmWindow>().Count() > 0, "New Algorithm Window was not closed");
            });
        }

        public void TestCreateAlgorithmRun1()
        {
            var w = GlobalManager.Instance.MainWindow;
            Dispatcher.UIThread.Invoke(() =>
            {
                var b = w.FindControl<Button>("PlayButton");
                Expect.False(b.IsEffectivelyEnabled, "Play button was enabled.");
                Helpers.CreateAlgorithmRun(20, typeof(OneMax), typeof(LinkageTreeFOS), typeof(IterationTermination), "10", typeof(MIP));
            });

            Thread.Sleep(1000);

            Dispatcher.UIThread.Invoke(() =>
            {
                var b = w.FindControl<Button>("PlayButton");
                Expect.True(b.IsEffectivelyEnabled, "Play button was not enabled. (1)");
                var i = Helpers.Find<IterationDetailWindow>();
                Expect.True(i.IsEffectivelyVisible, "Iteration details not visible.");
                Expect.Equal(typeof(MIPIterationViewModel), i.Content.GetType(), "Iteration Details has wrong type");
            });
        }
 
        public void TestCreateAlgorithmRun2()
        {
            var w = GlobalManager.Instance.MainWindow;
            Dispatcher.UIThread.Invoke(() =>
            {
                var b = w.FindControl<Button>("PlayButton");
                Expect.True(b.IsEffectivelyEnabled, "Play button was not enabled. (2)");
                Helpers.CreateAlgorithmRun(20, typeof(OneMax), typeof(LinkageTreeFOS), typeof(IterationTermination), "10", typeof(ROMEA));
            });

            Thread.Sleep(1000);

            Dispatcher.UIThread.Invoke(() =>
            {
                var b = w.FindControl<Button>("PlayButton");
                Expect.True(b.IsEffectivelyEnabled, "Play button was not enabled. (3)");
                var i = Helpers.Find<IterationDetailWindow>();
                Expect.True(i.IsEffectivelyVisible, "Iteration details not visible. (1)");
                Expect.Equal(typeof(ROMIterationViewModel), i.Content.GetType(), "Iteration Details has wrong type. (1)");
            });
        }

        public void TestPopulationDepiction()
        {
            var w = GlobalManager.Instance.MainWindow;
            Dispatcher.UIThread.Invoke(() =>
            {
                bool foundPopulationBlock = false;
                foreach (var c in w.GetLogicalDescendants().OfType<Control>())
                {
                    var control = (Control)c;
                    if (control.DataContext != null && control.DataContext.GetType().IsSubclassOf(typeof(PopulationContainerViewModelBase)))
                    {
                        if (control.DataContext.GetType().Equals(typeof(PopulationBlock)))
                        {
                            foundPopulationBlock = true;
                        }
                        else
                        {
                            Expect.Fail("Should only show PopulationBlocks");
                        }
                    }
                }
                if (!foundPopulationBlock)
                {
                    Expect.Fail("Should show a Population Block");
                }
            });
        }
 
        public void TestNextIteration()
        {
            var w = GlobalManager.Instance.MainWindow;
            Dispatcher.UIThread.Invoke(() =>
            {
                Helpers.ChangeAnimationModus(1);
            });

            Thread.Sleep(100);

            Dispatcher.UIThread.Invoke(() =>
            {
                Helpers.NextIteration();
            });

            Thread.Sleep(100);

            Dispatcher.UIThread.Invoke(() =>
            {
                Helpers.CloseAllExceptMain();
            });

            Thread.Sleep(100);

            var app = AvaloniaApp.GetApp();
            Dispatcher.UIThread.Invoke(() =>
            {
                Expect.Equal(1, app.Windows.Count, "More than one window open.");
            });

        }
 
        public void TestOtherAlgorithms()
        {
            var w = GlobalManager.Instance.MainWindow;
            Dispatcher.UIThread.Invoke(() =>
            {
                Helpers.ChangeAnimationModus(0);
            });

        
            Thread.Sleep(100);
            Dispatcher.UIThread.Invoke(() =>
            {
                Expect.Equal(0, Helpers.GetAnimationModus(), "Animation modus is wrong.");
                Helpers.CreateAlgorithmRun(14, typeof(DeceptiveTrap), typeof(MarginalProductFOS), typeof(IterationTermination), "10", typeof(P3), typeof(HillClimber), fitnessArg: "7");
            });

            Thread.Sleep(500);

            Dispatcher.UIThread.Invoke(() =>
            {
                var i = Helpers.Find<IterationDetailWindow>();
                Expect.True(i.IsEffectivelyVisible, "Iteration details not visible. (2)");
                Expect.Equal(typeof(MIPIterationViewModel), i.Content.GetType(), "Iteration Details has wrong type. (2)");
                Helpers.CreateAlgorithmRun(16, typeof(HIFF), typeof(UnivariateFOS), typeof(IterationTermination), "10", typeof(LocalSearchGOMEA), typeof(HillClimber), populationSize: 10);
            });

            Thread.Sleep(500);

            Dispatcher.UIThread.Invoke(() =>
            {
                var i = Helpers.Find<IterationDetailWindow>();
                Expect.True(i.IsEffectivelyVisible, "Iteration details not visible. (3)");
                Expect.Equal(typeof(GOMIterationViewModel), i.Content.GetType(), "Iteration Details has wrong type. (3)");
                Helpers.CreateAlgorithmRun(16, typeof(OneMax), typeof(UnivariateFOS), typeof(IterationTermination), "10", typeof(LocalSearchROMEA), typeof(HillClimber), populationSize: 10);
            });

            Thread.Sleep(500);

            Dispatcher.UIThread.Invoke(() =>
            {
                var i = Helpers.Find<IterationDetailWindow>();
                Expect.True(i.IsEffectivelyVisible, "Iteration details not visible. (4)");
                Expect.Equal(typeof(ROMIterationViewModel), i.Content.GetType(), "Iteration Details has wrong type. (4)");
                Helpers.CreateAlgorithmRun(16, typeof(HIFF), typeof(UnivariateFOS), typeof(IterationTermination), "10", typeof(GOMEA), populationSize: 10);
            });



            Thread.Sleep(500);

            Dispatcher.UIThread.Invoke(() =>
            {
                var i = Helpers.Find<IterationDetailWindow>();
                Expect.True(i.IsEffectivelyVisible, "Iteration details not visible. (5)");
                Expect.Equal(typeof(GOMIterationViewModel), i.Content.GetType(), "Iteration Details has wrong type. (5)");
            });

        }

        public void TestPopulationDepictions()
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                foreach (var c in Helpers.Find<MainWindow>().GetLogicalDescendants().OfType<PopulationDepictionViewBase>())
                {
                    if (c.GetType().Equals(typeof(PopulationBlocksView))) {
                        Expect.True(c.IsEffectivelyVisible);
                    }
                    else
                    {
                        Expect.False(c.IsEffectivelyVisible);
                    }
                }
                Helpers.ChangePopulationDepictionModus(1);
            });
            Thread.Sleep(1000);
            Dispatcher.UIThread.Invoke(() =>
            {
                CheckBox box = null;
                foreach (var c in GlobalManager.Instance.MainWindow.GetLogicalDescendants().OfType<PopulationDepictionViewBase>())
                {
                    if (c.GetType().Equals(typeof(PopulationGraphsView)))
                    {
                        box = c.Find<CheckBox>("AverageCheck");
                        Expect.True(c.IsEffectivelyVisible);
                    }
                    else
                    {
                        Expect.False(c.IsEffectivelyVisible);
                    }
                }

                foreach (var c in Helpers.Find<MainWindow>().GetLogicalDescendants().OfType<CartesianChart>())
                {
                    Expect.True(((PopulationGraph)c.DataContext).Series[0].IsVisible);
                    Expect.True(((PopulationGraph)c.DataContext).Series[1].IsVisible);
                    Expect.True(((PopulationGraph)c.DataContext).Series[2].IsVisible);
                    Expect.False(((PopulationGraph)c.DataContext).Series[3].IsVisible);
                }

                box.IsChecked = false;
            });

            Thread.Sleep(500);
            Dispatcher.UIThread.Invoke(() =>
            {
                CheckBox box = null;
                foreach (var c in GlobalManager.Instance.MainWindow.GetLogicalDescendants().OfType<PopulationDepictionViewBase>())
                {
                    if (c.GetType().Equals(typeof(PopulationGraphsView)))
                    {
                        box = c.Find<CheckBox>("MedianCheck");
                    }
                }

                foreach (var c in Helpers.Find<MainWindow>().GetLogicalDescendants().OfType<CartesianChart>())
                {
                    Expect.False(((PopulationGraph)c.DataContext).Series[0].IsVisible);
                    Expect.True(((PopulationGraph)c.DataContext).Series[1].IsVisible);
                    Expect.True(((PopulationGraph)c.DataContext).Series[2].IsVisible);
                    Expect.False(((PopulationGraph)c.DataContext).Series[3].IsVisible);
                }

                box.IsChecked = false;
            });

            Thread.Sleep(500);
            Dispatcher.UIThread.Invoke(() =>
            {
                CheckBox box = null;
                foreach (var c in GlobalManager.Instance.MainWindow.GetLogicalDescendants().OfType<PopulationDepictionViewBase>())
                {
                    if (c.GetType().Equals(typeof(PopulationGraphsView)))
                    {
                        box = c.Find<CheckBox>("MaximumCheck");
                    }
                }

                foreach (var c in Helpers.Find<MainWindow>().GetLogicalDescendants().OfType<CartesianChart>())
                {
                    Expect.False(((PopulationGraph)c.DataContext).Series[0].IsVisible);
                    Expect.False(((PopulationGraph)c.DataContext).Series[1].IsVisible);
                    Expect.True(((PopulationGraph)c.DataContext).Series[2].IsVisible);
                    Expect.False(((PopulationGraph)c.DataContext).Series[3].IsVisible);
                }

                box.IsChecked = false;
            });

            Thread.Sleep(500);
            Dispatcher.UIThread.Invoke(() =>
            {
                CheckBox box = null;
                foreach (var c in GlobalManager.Instance.MainWindow.GetLogicalDescendants().OfType<PopulationDepictionViewBase>())
                {
                    if (c.GetType().Equals(typeof(PopulationGraphsView)))
                    {
                        box = c.Find<CheckBox>("MinimumCheck");
                    }
                }

                foreach (var c in Helpers.Find<MainWindow>().GetLogicalDescendants().OfType<CartesianChart>())
                {
                    Expect.False(((PopulationGraph)c.DataContext).Series[0].IsVisible);
                    Expect.False(((PopulationGraph)c.DataContext).Series[1].IsVisible);
                    Expect.False(((PopulationGraph)c.DataContext).Series[2].IsVisible);
                    Expect.False(((PopulationGraph)c.DataContext).Series[3].IsVisible);
                }

                box.IsChecked = true;
            });

            Thread.Sleep(500);
            Dispatcher.UIThread.Invoke(() =>
            {
                foreach (var c in Helpers.Find<MainWindow>().GetLogicalDescendants().OfType<CartesianChart>())
                {
                    Expect.False(((PopulationGraph)c.DataContext).Series[0].IsVisible);
                    Expect.False(((PopulationGraph)c.DataContext).Series[1].IsVisible);
                    Expect.False(((PopulationGraph)c.DataContext).Series[2].IsVisible);
                    Expect.True(((PopulationGraph)c.DataContext).Series[3].IsVisible);
                }
                Helpers.ChangePopulationDepictionModus(2);
            });
            Thread.Sleep(1000);
            Dispatcher.UIThread.Invoke(() =>
            {
                foreach (var c in Helpers.Find<MainWindow>().GetLogicalDescendants().OfType<PopulationDepictionViewBase>())
                {
                    if (c.GetType().Equals(typeof(PopulationBarsView)))
                    {
                        Expect.True(c.IsEffectivelyVisible);
                    }
                    else
                    {
                        Expect.False(c.IsEffectivelyVisible);
                    }
                }
                Helpers.ChangePopulationDepictionModus(3);
            });

            Thread.Sleep(1000);
            Dispatcher.UIThread.Invoke(() =>
            {
                foreach (var c in Helpers.Find<MainWindow>().GetLogicalDescendants().OfType<PopulationDepictionViewBase>())
                {
                    if (c.GetType().Equals(typeof(PopulationBoxPlotView)))
                    {
                        Expect.True(c.IsEffectivelyVisible);
                    }
                    else
                    {
                        Expect.False(c.IsEffectivelyVisible);
                    }
                }
                Helpers.ChangePopulationDepictionModus(0);
            });
        }


    }
}