﻿using LLEAV.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.MIP.StateChange
{
    public class TerminationChange: IMIPStateChange
    {
        private bool _terminate;

        public TerminationChange(bool terminate)
        {
            _terminate = terminate;
        }

        public Tuple<IList<string>, Message> Apply(IterationData state, MIPVisualisationData visualisationData, bool onlyOperateOnData = false)
        {
            return new Tuple<IList<string>, Message>([],
                new Message("Termination criteria was " + (_terminate ? "" : "not ") + "met.", MessagePriority.IMPORTANT));
        }
    }
}
