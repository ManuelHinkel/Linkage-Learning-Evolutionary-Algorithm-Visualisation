using LLEAV.ViewModels;
using LLEAV.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.GOM.StateChange
{
    public class PresentedPopulationChanged: IGOMStateChange
    {
        private Population _population;

        public PresentedPopulationChanged(Population population)
        {
            _population = population;
        }
        public Tuple<IList<string>, string> Apply(IterationData state, GOMVisualisationData visualisationData)
        {
            GlobalManager.Instance.SelectPopulation(state, _population.PyramidIndex);

            visualisationData.Solutions = _population.Solutions.Select(s => new SolutionWrapper(s)).ToList();


            return new Tuple<IList<string>, string>(["Solutions"], "Changed the population currently viewed");
        }

    }
}
