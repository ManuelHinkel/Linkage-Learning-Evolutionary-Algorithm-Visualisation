using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using LLEAV.ViewModels;
using LLEAV.ViewModels.Windows;
using LLEAV.Views.Windows;

namespace LLEAVTest
{
    public class Helpers
    {

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


        public static async void CreateAlgorithmRun(int solutionLength, Type fitnessFunction, Type fosFunction,
            Type terminationCriteria, 
            string terminationArg,
            Type algorithm,
            Type localSearchFunction = null,
            Type growthFunction = null,
            int populationSize = 0,
            string fitnessArg = "")
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
            f.SelectedIndex = InddexOfType(fitnessFunction);

            var fos = newAlgorithmWindow.FindControl<ComboBox>("FOS");
            fos.SelectedIndex = InddexOfType(fosFunction);

            var t = newAlgorithmWindow.FindControl<ComboBox>("TerminationCriteria");
            t.SelectedIndex = InddexOfType(terminationCriteria);

            var tArg = newAlgorithmWindow.FindControl<TextBox>("TerminationArgument");
            tArg.Text = terminationArg;

            var fArg = newAlgorithmWindow.FindControl<TextBox>("FitnessArgument");
            fArg.Text = fitnessArg;

            var a = newAlgorithmWindow.FindControl<ComboBox>("Algorithm");
            a.SelectedIndex = InddexOfType(algorithm);


            if (localSearchFunction != null)
            {
                var l = newAlgorithmWindow.FindControl<ComboBox>("LocalSearchFunction");
                l.SelectedIndex = InddexOfType(localSearchFunction);
            }

            if (growthFunction != null)
            {
                var g = newAlgorithmWindow.FindControl<ComboBox>("GrowthFunction");
                g.SelectedIndex = InddexOfType(growthFunction);
            }

            if (populationSize > 0)
            {
                var p = newAlgorithmWindow.FindControl<NumericUpDown>("PopulationSize");
                p.Text = populationSize.ToString();
            }
            PressButton("Ok", newAlgorithmWindow);

        }

        private static int InddexOfType(Type t)
        {
            for (int i = 0; i < NewAlgorithmWindowViewModel.FitnessFunctions.Length; i++)
            {
                if (t.Equals(NewAlgorithmWindowViewModel.FitnessFunctions[i]))
                {
                    return i;
                }
            }
            for (int i = 0; i < NewAlgorithmWindowViewModel.LocalSearchFunctions.Length; i++)
            {
                if (t.Equals(NewAlgorithmWindowViewModel.LocalSearchFunctions[i]))
                {
                    return i;
                }
            }
            for (int i = 0; i < NewAlgorithmWindowViewModel.TerminationCriterias.Length; i++)
            {
                if (t.Equals(NewAlgorithmWindowViewModel.TerminationCriterias[i]))
                {
                    return i;
                }
            }
            for (int i = 0; i < NewAlgorithmWindowViewModel.Algorithms.Length; i++)
            {
                if (t.Equals(NewAlgorithmWindowViewModel.Algorithms[i]))
                {
                    return i;
                }
            }
            for (int i = 0; i < NewAlgorithmWindowViewModel.GrowthFunctions.Length; i++)
            {
                if (t.Equals(NewAlgorithmWindowViewModel.GrowthFunctions[i]))
                {
                    return i;
                }
            }
            for (int i = 0; i < NewAlgorithmWindowViewModel.FOSFunctions.Length; i++)
            {
                if (t.Equals(NewAlgorithmWindowViewModel.FOSFunctions[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public static void NextIteration()
        {
            var w = GlobalManager.Instance.MainWindow;

            PressButton("ForwardButton", w);
        }

        public static void ChangeAnimationModus(int modus)
        {
            var w = GlobalManager.Instance.MainWindow;

            var combobox = w.Find<ComboBox>("AnimationModus");
            combobox.SelectedIndex = modus;

            Assert.Equal(modus, combobox.SelectedIndex);
        }

        public static void ChangeBitDepictionModus(int modus)
        {
            var w = GlobalManager.Instance.MainWindow;

            var combobox = w.Find<ComboBox>("BitDepictionModus");
            combobox.SelectedIndex = modus;

            Assert.Equal(modus, combobox.SelectedIndex);
        }

        public static void ChangePopulationDepictionModus(int modus)
        {
            var w = GlobalManager.Instance.MainWindow;

            var combobox = w.Find<ComboBox>("depictionBox");
            combobox.SelectedIndex = modus;

            Assert.Equal(modus, combobox.SelectedIndex);
        }

        public static int GetAnimationModus()
        {
            var w = GlobalManager.Instance.MainWindow;

            var combobox = w.Find<ComboBox>("AnimationModus");
            return combobox.SelectedIndex;
        }

        public static int GetBitDepictionModus()
        {
            var w = GlobalManager.Instance.MainWindow;

            var combobox = w.Find<ComboBox>("BitDepictionModus");
            return combobox.SelectedIndex;
        }

        public static int GetPopulationDepictionModus()
        {
            var w = GlobalManager.Instance.MainWindow;

            var combobox = w.Find<ComboBox>("depictionBox");
            return combobox.SelectedIndex;
        }


        public static void CloseAllExceptMain()
        {
            var app = AvaloniaApp.GetApp();
            app.Windows.ToList().ForEach(w => { if (!w.GetType().Equals(typeof(MainWindow))) { w.Close(); } });
        }

        public static async void CloseWindow<T>() where T : Window
        {
            var app = AvaloniaApp.GetApp();
            app.Windows.ToList().ForEach(w => { if (w.GetType().Equals(typeof(T))) { w.Close(); } });
        }
    }
}
