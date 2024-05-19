using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.GOM.StateChange
{
    public class TerminationChange: IGOMStateChange
    {
        private bool _terminate;

        public TerminationChange(bool terminate)
        {
            _terminate = terminate;
        }

        public Tuple<IList<string>, string> Apply(IterationData state, GOMVisualisationData visualisationData)
        {
            return new Tuple<IList<string>, string>([], "Termination criteria was " + (_terminate ? "" : "not ") + "met.");
        }
    }
}
