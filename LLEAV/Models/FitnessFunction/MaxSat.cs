using Avalonia.Controls.Shapes;
using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.FitnessFunction
{
    public class Variable
    {
        public int BitStringIndex { get; private set; }

        public Variable(string bitStringIndex)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
            if (converter.IsValid(bitStringIndex))
            {
                this.BitStringIndex = int.Parse(bitStringIndex, CultureInfo.InvariantCulture);
            } else
            {
                throw new ArgumentException();
            }
        }

        public Variable(int bitStringIndex) 
        { 
            this.BitStringIndex = bitStringIndex;
        }

        public virtual bool Evaluate(BitList bitList)
        {
            return bitList.Get(BitStringIndex);
        }
    }

    public class NegatedVariable : Variable
    {
        public NegatedVariable(int bitStringIndex) : base(bitStringIndex)
        {
        }

        public NegatedVariable(string bitStringIndex) : base(bitStringIndex)
        {
        }

        public override bool Evaluate(BitList bitList)
        {
            return !bitList.Get(BitStringIndex);
        }
    }

    public class OrTerm
    {
        public List<Variable> Variables = new List<Variable>();

        public OrTerm(string term)
        {
            if (!(term.StartsWith("(") && term.EndsWith(")"))) {
                throw new ArgumentException();
            }
            term = term.Substring(1, term.Length - 2);
            string[] variables = term.Split("|");

            foreach (string variable in variables)
            {
                if (variable.StartsWith("!"))
                {
                    Variables.Add(new NegatedVariable(variable.Substring(1)));
                }
                else
                {
                    Variables.Add(new Variable(variable));
                }
            }
        }

        public bool Evaluate(BitList bitList)
        {
            bool value = false;
            foreach(var v in Variables)
            {
                value |= v.Evaluate(bitList);
            }
            return value;
        }
    }

    public class MaxSat : AFitnessFunction
    {
        public override string Depiction { get => "Max Sat"; }
        public override bool EnableArg => true;

        private List<OrTerm> _terms { get; set; }

        private string _arg;
        private int _maxPosition;

    
        public MaxSat()
        {}

        public override byte[] ConvertArgumentToBytes()
        {
            return Encoding.ASCII.GetBytes(_arg);
        }

        public override bool CreateArgumentFromBytes(byte[] bytes)
        {
            _arg = Encoding.ASCII.GetString(bytes);
            return CreateArgumentFromString(_arg);
        }

        public override bool CreateArgumentFromString(string arg)
        {
            _terms = new List<OrTerm>();
            _arg = String.Concat(arg.Where(c => !Char.IsWhiteSpace(c)));
            string[] orTerms = _arg.Split('&');

            foreach(string orTerm in orTerms)
            {
                try
                {
                    _terms.Add(new OrTerm(orTerm));
                } catch (Exception e)
                {
                    return false;
                }
            }
            foreach (var orTerm in _terms)
            {
                foreach(var v in orTerm.Variables)
                {
                    _maxPosition = Math.Max(_maxPosition, v.BitStringIndex);
                }
            }
            return true;
        }

        public override string GetArgValidationErrorMessage(string arg)
        {
            return "Input must be of form (0 | !1 | 4 | ...) & (!2 | 3 | ...) & ...";
        }

        public override double Fitness(Solution solution)
        {
            double sum = 0;

            foreach(var t in _terms)
            {
                if (t.Evaluate(solution.Bits))
                {
                    sum += 1;
                }
            }

            return sum;
        }

        public override string GetSolutionLengthValidationErrorMessage(int solutionLength)
        {
            return "Solutions need to be at least " + (_maxPosition + 1) + " bits long.";
        }

        public override bool ValidateSolutionLength(int solutionLength)
        {
            return solutionLength > _maxPosition;
        }
    }
}
