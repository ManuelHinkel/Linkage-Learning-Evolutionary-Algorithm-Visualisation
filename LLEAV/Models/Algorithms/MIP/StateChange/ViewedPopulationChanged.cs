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
    public class ViewedPopulationChanged: IMIPStateChange
    {
        private Population _population;

        public ViewedPopulationChanged(Population population)
        {
            _population = population;
        }
        public Tuple<IList<string>, Message> Apply(IterationData state, MIPVisualisationData visualisationData, bool onlyOperateOnData = false)
        {
            if (!onlyOperateOnData)
            {
                GlobalManager.Instance.SelectPopulation(state, _population.PyramidIndex);
            }

            visualisationData.ViewedPopulation = _population;

            return new Tuple<IList<string>, Message>([],
                new Message("Changed the population currently viewed", MessagePriority.INTERESTING));
        }

    }
}
