using LLEAV.Models;
using LLEAV.ViewModels;
using LLEAV.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.MIP.StateChange
{
    public class ActiveSolutionChange : IMIPStateChange
    {
        private Solution _activeSolution;
        public ActiveSolutionChange(Solution activeSolution) 
        { 
            _activeSolution = activeSolution;
        }
        public Tuple<IList<string>, Message> Apply(IterationData state, MIPVisualisationData visualisationData, bool onlyOperateOnData = false)
        {
            visualisationData.CurrentSolution = _activeSolution;

            return new Tuple<IList<string>, Message>(["CurrentSolution"], 
                new Message("Changed active solution to: \n" + _activeSolution.Bits, MessagePriority.INTERESTING));
        }
    }
}
