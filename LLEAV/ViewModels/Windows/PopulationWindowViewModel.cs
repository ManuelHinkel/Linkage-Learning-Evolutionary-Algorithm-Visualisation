using Avalonia.Threading;
using LLEAV.Models;
using LLEAV.Models.Tree;
using LLEAV.Util;
using LLEAV.ViewModels.Controls;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace LLEAV.ViewModels.Windows
{
    public class PopulationWindowViewModel : ViewModelBase
    {
        private static readonly string[] _colors = [
            "#ff0000",
            "#00ff00",
            "#00aaff",
            "#ffff00",
            "#ff00ff",
            "#00ffff",
        ];

        private static int _id;

        public AnimationModus AnimationModus
        {
            get => GlobalManager.Instance.AnimationModus; 
        }

        [Reactive]
        public Tree? Tree { get; private set; } = new Tree([], [], null);

        private bool[] _reservedColors = new bool[_colors.Length];

        private Population? _currentlyShownPopulation;

        public ObservableCollection<SolutionWrapper> Solutions { get; private set; }
            = new ObservableCollection<SolutionWrapper> { };

        private IList<SolutionWrapper> _wrappers = new List<SolutionWrapper>();

        private bool _stopLoading;
        private bool _threadRunning;

        public void SetPopulation(Population population)
        {
            if (_currentlyShownPopulation == population) return;

            _stopLoading = _threadRunning;

            //Wait for loading Thread to stop
            while (_threadRunning) Thread.Sleep(10);

            Thread t = new Thread(new ThreadStart(() => {
                LoadSolutions(population);
            }));
            t.Start();


            TreeLayouter layouter = new TreeLayouter();

            if (_currentlyShownPopulation != null && _currentlyShownPopulation.Equals(population.Previous)) 
            {
                Tree = layouter.UpdateTree(population.FOS, Tree!);

                // Reset _reserved colors and reserve, when already marked in tree
                _reservedColors = new bool[_colors.Length];

                foreach(Node n in Tree!.Nodes)
                {
                    for (int i = 0; i < _colors.Length; i++)
                    {
                        if (n.Color.Equals(_colors[i]))
                        {
                            _reservedColors[i] = true;
                            break;
                        }
                    }
                }

            } else
            {
                // Reset _reserved colors when a new tree is created.
                _reservedColors = new bool[_colors.Length];
                Tree = layouter.CalculateTree(population.FOS);
            }
            _currentlyShownPopulation = population;
        }

        private void LoadSolutions(Population population)
        {
            _threadRunning = true;
            _wrappers = population.Solutions.Select(s => new SolutionWrapper(s, GlobalManager.DEFAULT_WHITE, GlobalManager.GRAY)).ToList();
            Dispatcher.UIThread.InvokeAsync(() => {
                Solutions.Clear();
            });

            foreach (SolutionWrapper w in _wrappers)
            {
                if (_stopLoading)
                {
                    _stopLoading = false;
                    _threadRunning = false;
                    return;
                }
                Dispatcher.UIThread.InvokeAsync(() => {
                    Solutions.Add(w);
                });
                Thread.Sleep(50);
            }
            _threadRunning = false;
            _stopLoading = false;
        }

        public void ToggleCluster(Node node)
        {
            if (node.Cluster == null) return;
            SetMarkedStatus(node, string.IsNullOrEmpty(node.Color));
            ColorSolutions();
        }

        private void SetMarkedStatus(Node node, bool marked)
        {
            string color = "";
            if (!marked)
            {
                for (int i = 0; i < _colors.Length; i++)
                {
                    if (node.Color.Equals(_colors[i]))
                    {
                        _reservedColors[i] = false;
                        break;
                    }
                }

            }
            else
            {
                int colorIndex = GetReservedColor();

                if (colorIndex != -1)
                {
                    color = _colors[colorIndex];
                    _reservedColors[colorIndex] = true;
                }
                else
                {
                    color = GetNextColor();
                }
            }

            node.Color = color;
        }

        private void ColorSolutions()
        {
            if (Tree == null) return;

            foreach(SolutionWrapper wrapper in _wrappers)
            {
                wrapper.ClearColoring();
            }

            List<Node> dfs = [Tree!.Root];
            while (dfs.Count > 0)
            {
                Node current = dfs[0];
                dfs.RemoveAt(0);
                dfs.AddRange(current.Children);

                if (current.Cluster != null && !string.IsNullOrEmpty(current.Color))
                {
                    foreach (SolutionWrapper wrapper in _wrappers)
                    {
                        if (!GlobalManager.Instance.IsBarCodeDepiction)
                        {
                            wrapper.MarkCluster(current.Cluster, current.Color);
                        }
                        else
                        {
                            string dark = String.Concat(current.Color.Substring(1).Select(c => (Convert.ToInt32(c.ToString(), 16) / 2).ToString()));
                            dark = "#" + dark;
                            wrapper.MarkCluster(current.Color, dark, current.Cluster);
                        }
                    }
                }
            }
        }
 
        private int GetReservedColor()
        {
            for (int i = 0; i < _reservedColors.Length; i++)
            {
                if (_reservedColors[i] == false)
                {
                    return i;
                }
            }
            return -1;
        }
        private string GetNextColor()
        {
            double PHI = (1 + Math.Sqrt(5.0)) / 2.0;
            double n = _id * PHI - Math.Floor(_id * PHI);
            _id++;

            double hue = n * 360.0;

            return HSVConverter.FromHSV(hue, 0.8, 1);
        }

        public void MarkCluster(Cluster cluster)
        {
            Node toMark = null;
            foreach(Node n in Tree!.Nodes)
            {
                if (n.Cluster == null) continue;
                
                SetMarkedStatus(n, false);

                if (cluster.Equals(n.Cluster))
                {
                    toMark = n;
                }
            }
            if (toMark != null) 
            { 
                SetMarkedStatus(toMark, true);
            }
            ColorSolutions();
        }

        public void ChangeSolutionDepiction()
        {
            if (_currentlyShownPopulation == null) return;

            foreach(Node node in Tree!.Nodes)
            {
                SetMarkedStatus(node, false);
            }

            foreach (SolutionWrapper solutionWrapper in _wrappers)
            {
                solutionWrapper.IsBarCode = GlobalManager.Instance.IsBarCodeDepiction;
            }
        }
    }
}
