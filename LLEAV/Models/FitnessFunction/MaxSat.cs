using LLEAV.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.FitnessFunction
{
    public class Variable
    {
        protected int bitStringIndex;

        public Variable(int bitStringIndex) 
        { 
            this.bitStringIndex = bitStringIndex;
        }

        public virtual bool Evaluate(BitList bitList)
        {
            return bitList.Get(bitStringIndex);
        }
    }

    public class NegatedVariable : Variable
    {
        public NegatedVariable(int bitStringIndex) : base(bitStringIndex)
        {
        }

        public override bool Evaluate(BitList bitList)
        {
            return !bitList.Get(bitStringIndex);
        }
    }

    public class OrTerm
    {
        public List<Variable> Variables = new List<Variable>();

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
        private List<OrTerm> _terms;

        public MaxSat()
        {
            // 01 or 10 in positions 0 and 1 for both terms
            OrTerm t1 = new OrTerm();
            t1.Variables.Add(new Variable(0));
            t1.Variables.Add(new Variable(1));
            OrTerm t2 = new OrTerm();
            t2.Variables.Add(new NegatedVariable(0));
            t2.Variables.Add(new NegatedVariable(1));


            OrTerm t3 = new OrTerm();
            t3.Variables.Add(new Variable(2));

            OrTerm t4 = new OrTerm();
            t4.Variables.Add(new NegatedVariable(3));

            // x_4 | x_5 | !x_6
            OrTerm t5 = new OrTerm();
            t5.Variables.Add(new Variable(4));
            t5.Variables.Add(new Variable(5));
            t5.Variables.Add(new NegatedVariable(6));

            
            OrTerm t6 = new OrTerm();
            t6.Variables.Add(new Variable(7));
            t6.Variables.Add(new Variable(8));
            t6.Variables.Add(new Variable(9));

            OrTerm t7 = new OrTerm();
            t7.Variables.Add(new Variable(7));
            t7.Variables.Add(new Variable(8));
            t7.Variables.Add(new NegatedVariable(9));

            OrTerm t8 = new OrTerm();
            t8.Variables.Add(new Variable(7));
            t8.Variables.Add(new NegatedVariable(8));
            t8.Variables.Add(new Variable(9));

            OrTerm t9 = new OrTerm();
            t9.Variables.Add(new Variable(7));
            t9.Variables.Add(new NegatedVariable(8));
            t9.Variables.Add(new NegatedVariable(9));

            OrTerm t10 = new OrTerm();
            t10.Variables.Add(new NegatedVariable(7));
            t10.Variables.Add(new Variable(8));
            t10.Variables.Add(new Variable(9));

            OrTerm t11 = new OrTerm();
            t11.Variables.Add(new NegatedVariable(7));
            t11.Variables.Add(new Variable(8));
            t11.Variables.Add(new NegatedVariable(9));

            OrTerm t12 = new OrTerm();
            t12.Variables.Add(new NegatedVariable(7));
            t12.Variables.Add(new NegatedVariable(8));
            t12.Variables.Add(new Variable(9));

            _terms = [
                t1,
                t2,
                t3,
                t4,
                t5,
                t6,
                t7,
                t8,
                t9,
                t10,
                t11,
                t12,
            ];
        }

        public override string Depiction { get; } = "Max Sat";

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

        public override string GetValidationErrorMessage(int solutionLength)
        {
            return "Solutions need to be 10 bits long.";
        }

        public override bool ValidateSolutionLength(int solutionLength)
        {
            return solutionLength == 10;
        }
    }
}
