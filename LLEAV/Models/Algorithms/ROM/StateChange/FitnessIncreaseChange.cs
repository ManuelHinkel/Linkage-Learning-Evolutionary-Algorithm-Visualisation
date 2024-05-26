using LLEAV.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.ROM.StateChange
{
    public class FitnessIncreaseChange: IROMStateChange
    {
        private Solution _p0;
        private Solution _p1;

        public FitnessIncreaseChange(Solution p0, Solution p1)
        {
            _p0 = p0;
            _p1 = p1;
        }
        public Tuple<IList<string>, Message> Apply(IterationData state, ROMVisualisationData visualisationData, bool onlyOperateOnData = false)
        {
            visualisationData.IsFitnessIncreasing = true;
            visualisationData.CurrentDonor1 = _p0;
            visualisationData.CurrentDonor2 = _p1;
            return new Tuple<IList<string>, Message>(["IsFitnessIncreasing", "CurrentDonor1", "CurrentDonor2"],
               new Message("Fitness Increased. Applied to parents resulting in:\n " + _p0.Bits + "\n and: " + _p1.Bits,
               MessagePriority.IMPORTANT));
        }
    }
}
