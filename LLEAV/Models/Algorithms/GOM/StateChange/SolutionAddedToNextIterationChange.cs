using LLEAV.Models.Algorithms.MIP.StateChange;
using LLEAV.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.GOM.StateChange
{
    public class SolutionAddedToNextIterationChange: IGOMStateChange
    {
        private Solution _solution;
        public SolutionAddedToNextIterationChange(Solution solution)
        {
            _solution = solution;
        }
        public Tuple<IList<string>, Message> Apply(IterationData state, GOMVisualisationData visualisationData, bool onlyOperateOnData = false)
        {

            visualisationData.NextIteration.Add(new SolutionWrapper(_solution));

            return new Tuple<IList<string>, Message>(["NextIterationAdded"], 
                new Message("Added solution to the next iteration.", MessagePriority.IMPORTANT));
        }
    }
}
