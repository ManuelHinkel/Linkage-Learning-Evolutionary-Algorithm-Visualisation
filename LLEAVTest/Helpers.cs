using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using LLEAV;
using LLEAV.Models.GrowthFunction;
using LLEAV.ViewModels;
using LLEAV.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAVTest
{
    public class Helpers
    {
        public static async Task WaitFor(Func<bool> condition, int delayMs = 50, int maxAttempts = 20)
        {
            for (var i = 0; i < maxAttempts; i++)
            {
                await Task.Delay(delayMs);

                if (condition())
                {
                    return;
                }
            }
            Assert.Fail("Condition failed: " + condition.ToString());
        }

        public static Window Find<T>() where T: Window {

            var app = AvaloniaApp.GetApp();
            return app.Windows.OfType<T>().Single();
        } 

        public static void PressButton(string name, Window w)
        {
            var b = w.FindControl<Button>(name);

            Assert.True(b.IsVisible);

            b.Command.Execute(null);
        }

        public static void CreateAlgorithmRun(int solutionLength, int fitnessFunction, int fosFunction, 
            int terminationCriteria, 
            string terminationArg,
            int algorithm,
            int localSearchFunction = -1,
            int growthFunction = -1,
            int populationSize = 0)
        {
            var app = AvaloniaApp.GetApp();

            var w = GlobalManager.Instance.MainWindow;

            var file = w.FindControl<MenuItem>("File");

            file.Open();

            var n = w.FindControl<MenuItem>("NewFile");
            var clickEvent = new RoutedEventArgs(MenuItem.ClickEvent);
            n.RaiseEvent(clickEvent);

            var newAlgorithmWindow = Find<NewAlgorithmWindow>();

            var s = newAlgorithmWindow.FindControl<NumericUpDown>("SolutionLength");
            s.Text = solutionLength.ToString();

            var f = newAlgorithmWindow.FindControl<ComboBox>("FitnessFunction");
            f.SelectedIndex = fitnessFunction;

            var fos = newAlgorithmWindow.FindControl<ComboBox>("FOS");
            fos.SelectedIndex = fosFunction;

            var t = newAlgorithmWindow.FindControl<ComboBox>("TerminationCriteria");
            t.SelectedIndex = terminationCriteria;

            var tArg = newAlgorithmWindow.FindControl<TextBox>("TerminationArgument");
            tArg.Text = terminationArg;

            var a = newAlgorithmWindow.FindControl<ComboBox>("Algorithm");
            a.SelectedIndex = algorithm;


            if (localSearchFunction > -1)
            {
                var l = newAlgorithmWindow.FindControl<ComboBox>("LocalSearchFunction");
                l.SelectedIndex = localSearchFunction;
            }

            if (growthFunction > -1)
            {
                var g = newAlgorithmWindow.FindControl<ComboBox>("GrowthFunction");
                g.SelectedIndex = growthFunction;
            }

            if (populationSize > 0)
            {
                var p = newAlgorithmWindow.FindControl<NumericUpDown>("PopulationSize");
                p.Text = populationSize.ToString();
            }


            PressButton("Ok", newAlgorithmWindow);
        }
    }
}
