using LLEAV.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.MIP.StateChange
{
    public class EvaluationChange: IMIPStateChange
    {
        private Solution[] _solutions;

        public EvaluationChange(Solution[] solutions)
        {
            _solutions = solutions;
        }

        public Tuple<IList<string>, Message> Apply(IterationData state, MIPVisualisationData visualisationData, bool onlyOperateOnData = false)
        {
            visualisationData.FitnessEvaluations += _solutions.Length;


            string s = "Evaluated Fitness " + _solutions.Length + " time" + (_solutions.Length>1?"s":"") + " for solutions:";
            foreach(var solution in _solutions)
            {
                s += "\n" + solution.ToString();
            }

            return new Tuple<IList<string>, Message>(["Evaluations"], new Message(s));
        }
    }
}
