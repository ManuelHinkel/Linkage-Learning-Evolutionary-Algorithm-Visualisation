using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLEAV.Models;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace LLEAV.ViewModels.Controls
{
    public class ColoredChar: ReactiveObject
    {
        public char Character { get; set; }
        [Reactive]
        public string Color { get; set; }

        public ColoredChar(char character, string color = GlobalManager.TEXT_COLOR)
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


        private bool _isBarCode;
        public bool IsBarCode
        {
            get => _isBarCode;
            set
            {
                this.RaiseAndSetIfChanged(ref _isBarCode, value);
                Spacing = value ? 0 : 5;

                if (Bits != null)
                {
                    ChangeBitVisualisation();
                }
            }
        }

        public int BarCodeWidth { get; }

        [Reactive]
        public int Spacing { get; set; }

        public string BackgroundColor { get; }

        private string _textColor { get; }

        public SolutionWrapper(Solution solution, string color = GlobalManager.TEXT_COLOR, string backgroundColor = "#00000000")
        {
            IsBarCode = GlobalManager.Instance.IsBarCodeDepiction;
            Solution = solution;
            BarCodeWidth = /*Math.Max(100 / solution.Bits.NumberBits, 1)*/10;
            _textColor = color;
            CreateColoredString();
            BackgroundColor = backgroundColor;
        }

        public void ClearColoring()
        {
            ChangeBitVisualisation();
        }
        public void MarkCluster(Cluster? cluster, string color) 
        {
            if (cluster == null) return;
            if (string.IsNullOrEmpty(color)) color = _textColor;
            for (int i = 0; i < Bits.Length; i++)
            {
                if (cluster.Mask.Get(i))
                {
                    Bits[i].Color = color;
                }
            }
        }

        public void MarkCluster(string colorFor1, string colorFor0, Cluster? cluster)
        {
            if (cluster == null) return;
            if (string.IsNullOrEmpty(colorFor1)) colorFor1 = GlobalManager.DEFAULT_WHITE;
            if (string.IsNullOrEmpty(colorFor0)) colorFor0 = GlobalManager.TEXT_COLOR;
            for (int i = 0; i < Bits.Length; i++)
            {
                if (cluster.Mask.Get(i))
                {
                    Bits[i].Color = Bits[i].Character == '1' ? colorFor1 : colorFor0;
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
                Bits[i] = new ColoredChar(Solution.Bits.Get(i) ? '1' : '0',_textColor);
                if (IsBarCode)
                {
                    Bits[i].Color = Bits[i].Character == '1' ? GlobalManager.DEFAULT_WHITE : GlobalManager.TEXT_COLOR;
                }
            }
        }

        private void ChangeBitVisualisation()
        {
            if (IsBarCode)
            {
                Bits.ToList().ForEach(b => b.Color = b.Character == '1' ? GlobalManager.DEFAULT_WHITE : GlobalManager.TEXT_COLOR);
            } else
            {
                Bits.ToList().ForEach(b => b.Color = _textColor);
            }
        }
    }

}
