using LLEAV.Models;
using LLEAV.ViewModels;
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
        public Tuple<IList<string>, string> Apply(IterationData state, MIPVisualisationData visualisationData, bool onlyOperateOnData = false)
        {
            visualisationData.CurrentSolution = _activeSolution;

            return new Tuple<IList<string>, string>(["CurrentSolution"], "Changed active solution to: \n" + _activeSolution.Bits);
        }
    }
}
