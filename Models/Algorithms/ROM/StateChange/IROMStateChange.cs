using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLEAV.Models.Algorithms;
using LLEAV.ViewModels;

namespace LLEAV.Models.Algorithms.ROM.StateChange
{
    public class ROMVisualisationData()
    {

    }
    public interface IROMStateChange : IStateChange
    {
        public void Apply(IterationData state, ROMVisualisationData visualisationData);
    }
}
