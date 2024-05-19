using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLEAV.Models;
using System.Diagnostics;

namespace LLEAV.ViewModels.Controls
{
    public class ColoredChar: ReactiveObject
    {
        public const string DEFAULT_COLOR = "#feedf2";
        public char Character { get; set; }
        [Reactive]
        public string Color { get; set; }

        public ColoredChar(char character, string color = DEFAULT_COLOR)
        {
            Character = character;
            Color = color;
        }
    }

    public class SolutionWrapper : ReactiveObject
    {
        public Solution Solution { get; private set; }
        public ColoredChar[] Bits { get; set; }
        public double Fitness { get => Solution.Fitness; }

        [Reactive]
        public bool Selected { get; set; }

        public SolutionWrapper(Solution solution)
        {
            Solution = solution;
            CreateColoredString();
        }

        public void MarkCluster(Cluster? cluster, string color) 
        {
            if (cluster == null) return;
            if (string.IsNullOrEmpty(color)) color = ColoredChar.DEFAULT_COLOR;
            for (int i = 0; i < Bits.Length; i++)
            {
                if (cluster.Mask.Get(i))
                {
                    Bits[i].Color = color;
                }
            }
        }

        private void CreateColoredString()
        {
            if (Solution == null || Solution.Bits == null) return;
            int numberOfBits = Solution.Bits.NumberBits;
            Bits = new ColoredChar[numberOfBits];

            for(int i = 0; i  < numberOfBits; i++) 
            {
                Bits[i] = new ColoredChar(Solution.Bits.Get(i) ? '1' : '0'); 
            }
        }
    }

}
