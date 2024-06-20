using LLEAV.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Linq;

namespace LLEAV.ViewModels.Controls
{
    /// <summary>
    /// Represents a single character, which can have different colors.
    /// </summary>
    public class ColoredChar : ReactiveObject
    {
        /// <summary>
        /// Gets or sets the character.
        /// </summary>
        public char Character { get; set; }

        /// <summary>
        /// Gets or sets the color of the character.
        /// </summary>
        [Reactive]
        public string Color { get; set; }

        /// <summary>
        /// Creates a new instance of a colored character.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="color">The color of the character.</param>
        public ColoredChar(char character, string color = GlobalManager.TEXT_COLOR)
        {
            Character = character;
            Color = color;
        }
    }

    /// <summary>
    /// Wrapper class for solutions used to render in the UI.
    /// </summary>
    public class SolutionWrapper : ReactiveObject
    {
        /// <summary>
        /// Gets the associated solution.
        /// </summary>
        public Solution Solution { get; private set; }

        /// <summary>
        /// Gets or sets the array of characters representing the bits.
        /// </summary>
        public ColoredChar[] Bits { get; set; }

        /// <summary>
        /// Gets the fitness of the solution.
        /// </summary>
        public double Fitness { get => Solution.Fitness; }

        /// <summary>
        /// Gets or sets, if the solution is selected.
        /// </summary>
        [Reactive]
        public bool Selected { get; set; }


        private bool _isBarCode;
        /// <summary>
        /// Gets or sets, if the solution should be shown as a barcode.
        /// </summary>
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

        /// <summary>
        /// Gets the barcode width.
        /// </summary>
        public int BarCodeWidth { get; }

        /// <summary>
        /// Gets or sets the spacing of characters.
        /// </summary>
        [Reactive]
        public int Spacing { get; set; }

        /// <summary>
        /// Gets the background color of the solution.
        /// </summary>
        public string BackgroundColor { get; }

        private string _textColor { get; }

        /// <summary>
        /// Creates a new instance of the solution wrapper with a solution, its text color and background color.
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="color"></param>
        /// <param name="backgroundColor"></param>
        public SolutionWrapper(Solution solution, string color = GlobalManager.TEXT_COLOR, string backgroundColor = "#00000000")
        {
            IsBarCode = GlobalManager.Instance.IsBarCodeDepiction;
            Solution = solution;
            BarCodeWidth = 10;
            _textColor = color;
            CreateColoredString();
            BackgroundColor = backgroundColor;
        }

        /// <summary>
        /// Clears the coloring of the characters.
        /// </summary>
        public void ClearColoring()
        {
            ChangeBitVisualisation();
        }

        /// <summary>
        /// Colors character of the cluster.
        /// </summary>
        /// <param name="cluster">The cluster for which the characters are marked.</param>
        /// <param name="color">The color used to mark the characters.</param>
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

        /// <summary>
        /// Colors character of the cluster in barcode depcition mode.
        /// </summary>
        /// <param name="colorFor1">The color used to mark the '1'.</param>
        /// <param name="colorFor0">The color used to mark the '0'.</param>
        /// <param name="cluster">The cluster for which the characters are marked.</param>
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

            for (int i = 0; i < numberOfBits; i++)
            {
                Bits[i] = new ColoredChar(Solution.Bits.Get(i) ? '1' : '0', _textColor);
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
            }
            else
            {
                Bits.ToList().ForEach(b => b.Color = _textColor);
            }
        }
    }

}
