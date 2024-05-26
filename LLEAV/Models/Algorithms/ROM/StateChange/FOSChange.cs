using LLEAV.ViewModels;
using LLEAV.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.ROM.StateChange
{
    public class FOSChange: IROMStateChange
    {

        private Population _population;
        public FOSChange(Population population)
        {
            _population = population;
        }
        public Tuple<IList<string>, Message> Apply(IterationData state, ROMVisualisationData visualisationData, bool onlyOperateOnData = false)
        {
           
            state.Populations[_population.PyramidIndex] = _population;

            return new Tuple<IList<string>, Message>([],
                new Message("Changed FOS of population.", MessagePriority.INTERESTING));
        }

    }
}
