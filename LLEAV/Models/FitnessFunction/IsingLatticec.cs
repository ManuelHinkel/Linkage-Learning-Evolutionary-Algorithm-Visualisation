using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.FitnessFunction
{
    public class IsingLattice : AFitnessFunction
    {
        public override string Depiction { get => "Ising Lattice\n(Columns: " + _columns + " Rows: " + _rows + ")"; }

        public override bool EnableArg => true;

        private int _rows;
        private int _columns;

        public override byte[] ConvertArgumentToBytes()
        {
            byte[] bytes = new byte[8];
            ByteUtil.WriteIntToBuffer(_columns, bytes, 0);
            ByteUtil.WriteIntToBuffer(_rows, bytes, 4);
            return bytes;
        }

        public override bool CreateArgumentFromBytes(byte[] bytes)
        {
            _columns = BitConverter.ToInt32(bytes, 0);
            _rows = BitConverter.ToInt32(bytes, 4);
            return true;
        }

        public override bool CreateArgumentFromString(string arg)
        {
            string[] tokens = arg.Split(' ');
            tokens = tokens.Where(c => !String.IsNullOrWhiteSpace(c)).ToArray();

            if (tokens.Length != 2) { return false; }

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
            if (converter.IsValid(tokens[0]))
            {
                _columns = (int)converter.ConvertTo(tokens[0], typeof(int));
            }
            if (converter.IsValid(tokens[1]))
            {
                _rows = (int)converter.ConvertTo(tokens[1], typeof(int));
            }
            return _columns > 1 && _rows > 1;
        }

        public override string GetArgValidationErrorMessage(string arg)
        {
            return "Argument must be of form: <#columns> <#rows>\nBoth Arguments must be greater than 1.";
        }

        public override string GetSolutionLengthValidationErrorMessage(int solutionLength)
        {
            return "Solution need to be exactly " + _columns * _rows + " bits long.";
        }

        public override bool ValidateSolutionLength(int solutionLength)
        {
            return solutionLength == _columns * _rows;
        }

        public override double Fitness(Solution solution)
        {
            BitList bits = solution.Bits;
            double fitness = 0;

            for(int x = 0; x < _columns; x++)
            {
                for (int y = 0; y < _rows; y++)
                {
                    int index = IndexOf(x, y);
                    int indexLeft = IndexOf((x + 1) % _columns, y);
                    int indexDown = IndexOf(x, (y + 1) % _rows);
                    fitness += bits.Get(index) == bits.Get(indexLeft) ? 1 : 0;
                    fitness += bits.Get(index) == bits.Get(indexDown) ? 1 : 0;
                }
            }

            return fitness;
        }

        private int IndexOf(int column, int row)
        {
            return column + _columns * row;
        }

    }
}
