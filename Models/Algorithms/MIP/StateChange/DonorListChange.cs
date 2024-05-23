using LLEAV.Models;
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
    public class DonorListChange : IMIPStateChange
    {

        private Population _population;
        public DonorListChange(Population population) 
        { 
            _population = population;
        }

        public Tuple<IList<string>, Message> Apply(IterationData state, MIPVisualisationData visualisationData, bool onlyOperateOnData = false)
        {
            visualisationData.CurrentSolution = null;
            visualisationData.CurrentDonor = null;
            visualisationData.Merged = null;
            visualisationData.ViewedPopulation = _population;
            visualisationData.ActivePopulation = _population;
            visualisationData.Donors = _population.Solutions.Select(s => new SolutionWrapper(s)).ToList();

            return new Tuple<IList<string>, Message>(["Donors", "Merged", "CurrentDonor", "CurrentSolution"], 
                new Message("Changed the population and donor list.",MessagePriority.IMPORTANT));
        }

    }
}
