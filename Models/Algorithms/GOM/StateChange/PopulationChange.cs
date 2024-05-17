using LLEAV.Models;
using LLEAV.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.GOM.StateChange
{
    public class PopulationChange : IGOMStateChange
    {
        public PopulationChange(Population population) { }

        public void Apply(IterationData state, GOMVisualisationData visualisationData)
        {
            throw new NotImplementedException();
        }

        public void Revert(IterationData state, GOMVisualisationData visualisationData)
        {
            throw new NotImplementedException();
        }
    }
}
