using LLEAV.ViewModels;
using LLEAV.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.MIP.StateChange
{
    public class SolutionAddedToNextPopulationChange : IMIPStateChange
    {

        private Population _population;
        public SolutionAddedToNextPopulationChange(Population population) 
        { 
            _population = population;
        }
        public Tuple<IList<string>, Message> Apply(IterationData state, MIPVisualisationData visualisationData, bool onlyOperateOnData = false)
        {
           
            if (state.Populations.Count < _population.PyramidIndex) { throw new Exception("Wrong execution of operations, can't skip a population."); }
            else if (state.Populations.Count == _population.PyramidIndex)
            {
                state.Populations.Add(_population);
            } else
            {
                state.Populations[_population.PyramidIndex] = _population;
            }

            return new Tuple<IList<string>, Message>([], 
                new Message("Added solution(s) to the next population.", MessagePriority.INTERESTING));
        }

    }
}
