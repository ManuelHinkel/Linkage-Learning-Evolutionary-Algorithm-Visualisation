﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.ROM.StateChange
{
    public class TerminationChange: IROMStateChange
    {
        private bool _terminate;

        public TerminationChange(bool terminate)
        {
            _terminate = terminate;
        }

        public Tuple<IList<string>, string> Apply(IterationData state, ROMVisualisationData visualisationData, bool onlyOperateOnData = false)
        {
            return new Tuple<IList<string>, string>([], "Termination criteria was " + (_terminate ? "" : "not ") + "met.");
        }
    }
}
