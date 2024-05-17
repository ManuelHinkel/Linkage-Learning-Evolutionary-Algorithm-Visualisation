using LLEAV.Models;
using LLEAV.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.ROM.StateChange
{
    public class CrossoverChange : IROMStateChange
    {
        public CrossoverChange(Cluster cluster) { }
        public void Apply(IterationData state, ROMVisualisationData visualisationData)
        {
            throw new NotImplementedException();
        }

        public void Revert(IterationData state, ROMVisualisationData visualisationData)
        {
            throw new NotImplementedException();
        }
    }
}
