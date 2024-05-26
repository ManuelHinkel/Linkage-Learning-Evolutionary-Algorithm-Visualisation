using LLEAV.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.GOM.StateChange
{
    public class ApplyCrossoverChange : IGOMStateChange
    {
        private Solution _current;

        public ApplyCrossoverChange(Solution current)
        {
            _current = current;
        }

        public Tuple<IList<string>, Message> Apply(IterationData state, GOMVisualisationData visualisationData, bool onlyOperateOnData = false)
        {
            visualisationData.IsApplyingCrossover = true;
            visualisationData.CurrentSolution = _current;
            return new Tuple<IList<string>, Message>(["IsApplyingCrossover", "CurrentSolution"], 
                new Message("Applied the merge.", MessagePriority.IMPORTANT));
        }
    }
}
