using LLEAV.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.ROM.StateChange
{
    public class EvaluationChange : IROMStateChange
    {
        private Solution[] _solutions;

        public EvaluationChange(Solution[] solutions)
        {
            _solutions = solutions;
        }
        public Tuple<IList<string>, Message> Apply(IterationData state, ROMVisualisationData visualisationData, bool onlyOperateOnData = false)
        {
            visualisationData.FitnessEvaluations += _solutions.Length;


            string s = "Evaluated Fitness " + _solutions.Length + " time" + (_solutions.Length > 1 ? "s" : "") + " for solutions:";
            foreach (var solution in _solutions)
            {
                s += "\n" + solution.ToString();
            }

            return new Tuple<IList<string>, Message>(["Evaluations"], new Message(s));
        }
    }
}
