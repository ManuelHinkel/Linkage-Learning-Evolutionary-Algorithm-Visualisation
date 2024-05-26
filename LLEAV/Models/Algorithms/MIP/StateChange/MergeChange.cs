using LLEAV.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.MIP.StateChange
{
    public class MergeChange : IMIPStateChange
    {

        private Solution _merged;

        public MergeChange(Solution merged)
        {
            _merged = merged;
        }
        public Tuple<IList<string>, Message> Apply(IterationData state, MIPVisualisationData visualisationData, bool onlyOperateOnData = false)
        {

            visualisationData.IsMerging = true;
            visualisationData.Merged = _merged;
            return new Tuple<IList<string>, Message>(["IsMerging", "Merged"],
                new Message("Merged the parents resulting in: \n" + _merged.Bits));
        }

    }
}
