using LLEAV.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.ROM.StateChange
{
    public class FitnessDecreaseChange : IROMStateChange
    {
        private Solution _o0;
        private Solution _o1;

        public FitnessDecreaseChange(Solution o0, Solution o1)
        {
            _o0 = o0;
            _o1 = o1;
        }
        public Tuple<IList<string>, Message> Apply(IterationData state, ROMVisualisationData visualisationData, bool onlyOperateOnData = false)
        {
            visualisationData.IsFitnessDecreasing = true;
            visualisationData.CurrentSolution1 = _o0;
            visualisationData.CurrentSolution2 = _o1;
            return new Tuple<IList<string>, Message>(["IsFitnessDecreasing", "CurrentSolution1", "CurrentSolution2"],
               new Message("Fitness Decreased. Reverted solutions:\n " + _o0.Bits + "\n and: " + _o1.Bits));
        }
    }
}
