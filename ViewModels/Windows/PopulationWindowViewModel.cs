using DynamicData;
using DynamicData.Kernel;
using LLEAV.Models;
using LLEAV.Models.Tree;
using LLEAV.Util;
using LLEAV.ViewModels.Controls;
using LLEAV.ViewModels.Controls.PopulationDepictions;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace LLEAV.ViewModels.Windows
{
    public class PopulationWindowViewModel : ViewModelBase
    {
        private static readonly string[] _colors = [
            "#ff0000",
            "#00ff00",
            "#0000ff",
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
        public Tree? Tree { get; private set; } = new Tree([], []);

        private bool[] _reservedColors = new bool[_colors.Length];

        private Population? currentlyShownPopulation;

        public ObservableCollection<SolutionWrapper> Solutions { get; private set; }
            = new ObservableCollection<SolutionWrapper> { };

        public void SetPopulation(Population population)
        {
            if (currentlyShownPopulation == population) return;

            Solutions.Clear();
            Solutions.AddRange(population.Solutions.Select(s => new SolutionWrapper(s)));


            TreeLayouter layouter = new TreeLayouter();

            if (currentlyShownPopulation != null && currentlyShownPopulation.Equals(population.Previous)) 
            {
                layouter.UpdateTree(population.FOS, Tree!);

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
            currentlyShownPopulation = population;
        }

        public void ToggleCluster(Node node)
        {
            if (node.Cluster == null) return;

            SetMarkedStatus(node, string.IsNullOrEmpty(node.Color));
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
            foreach (SolutionWrapper solutionWrapper in Solutions)
            {
                solutionWrapper.MarkCluster(node.Cluster, color);
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
        }
    }
}
