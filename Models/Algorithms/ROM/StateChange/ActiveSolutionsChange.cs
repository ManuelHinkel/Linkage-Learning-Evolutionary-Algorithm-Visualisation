using LLEAV.ViewModels;
using System;
using System.Collections.Generic;

namespace LLEAV.Models.Algorithms.ROM.StateChange
{
    public class ActiveSolutionsChange : IROMStateChange
    {
        private Solution _o0;
        private Solution _o1;
        private Solution _p0;
        private Solution _p1;
        public ActiveSolutionsChange(Solution o0, Solution o1, Solution p0, Solution p1)
        {
            _o0 = o0;
            _o1 = o1;
            _p0 = p0;
            _p1 = p1;
        }
        public Tuple<IList<string>, string> Apply(IterationData state, ROMVisualisationData visualisationData)
        {
            visualisationData.CurrentSolution1 = _o0;
            visualisationData.CurrentSolution2 = _o1;
            visualisationData.CurrentDonor1 = _p0;
            visualisationData.CurrentDonor2 = _p1;

            return new Tuple<IList<string>, string>(["CurrentDonor1", "CurrentDonor2", "CurrentSolution1", "CurrentSolution2"], "Changed active solutions");
        }
    }
}
