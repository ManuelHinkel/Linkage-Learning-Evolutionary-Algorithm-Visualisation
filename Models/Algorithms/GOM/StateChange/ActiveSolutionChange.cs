using LLEAV.ViewModels;
using System;

namespace LLEAV.Models.Algorithms.GOM.StateChange
{
    public class ActiveSolutionChange : IGOMStateChange
    {
        public ActiveSolutionChange(Solution activeSolution) { }
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
