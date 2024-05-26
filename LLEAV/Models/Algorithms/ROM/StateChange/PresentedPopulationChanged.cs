using LLEAV.ViewModels;
using LLEAV.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.ROM.StateChange
{
    public class PresentedPopulationChanged: IROMStateChange
    {
        private Population _population;

        public PresentedPopulationChanged(Population population)
        {
            _population = population;
        }
        public Tuple<IList<string>, Message> Apply(IterationData state, ROMVisualisationData visualisationData, bool onlyOperateOnData = false)
        {
            if (!onlyOperateOnData)
            {
                GlobalManager.Instance.SelectPopulation(state, 0);
            }

            visualisationData.Solutions = _population.Solutions.Select(s => new SolutionWrapper(s)).ToList();


            return new Tuple<IList<string>, Message>(["Solutions"],
                new Message("Changed the population currently viewed", MessagePriority.INTERESTING));
        }

    }
}
