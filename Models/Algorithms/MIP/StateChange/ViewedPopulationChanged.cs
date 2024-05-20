using LLEAV.ViewModels;
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
        public Tuple<IList<string>, string> Apply(IterationData state, MIPVisualisationData visualisationData, bool onlyOperateOnData = false)
        {
            if (!onlyOperateOnData)
            {
                GlobalManager.Instance.SelectPopulation(state, _population.PyramidIndex);
            }

            visualisationData.ViewedPopulation = _population;

            return new Tuple<IList<string>, string>([], "Changed the population currently viewed");
        }

    }
}
