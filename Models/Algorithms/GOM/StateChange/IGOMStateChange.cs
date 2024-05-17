using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLEAV.Models.Algorithms;
using LLEAV.ViewModels;

namespace LLEAV.Models.Algorithms.GOM.StateChange
{
    public class GOMVisualisationData()
    {

    }
    public interface IGOMStateChange : IStateChange
    {
        public void Apply(IterationData state, GOMVisualisationData visualisationData);
    }
}
