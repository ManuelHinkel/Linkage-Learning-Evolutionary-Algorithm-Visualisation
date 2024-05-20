﻿using LLEAV.ViewModels;
using System;
using System.Collections.Generic;

namespace LLEAV.Models.Algorithms.GOM.StateChange
{
    public class ActiveSolutionChange : IGOMStateChange
    {
        private Solution _activeSolution;
        public ActiveSolutionChange(Solution activeSolution)
        {
            _activeSolution = activeSolution;
        }
        public Tuple<IList<string>, string> Apply(IterationData state, GOMVisualisationData visualisationData, bool onlyOperateOnData = false)
        {
            visualisationData.CurrentSolution = _activeSolution;
            visualisationData.Merged = null;
            visualisationData.CurrentDonor = null;

            return new Tuple<IList<string>, string>(["CurrentSolution", "CurrentDonor", "Merged"], "Changed active solution to: \n" + _activeSolution.Bits);
        }
    }
}
