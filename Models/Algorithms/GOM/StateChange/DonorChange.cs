using LLEAV.Models;
using LLEAV.Models.Algorithms.MIP.StateChange;
using LLEAV.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.GOM.StateChange
{
    public class DonorChange : IGOMStateChange
    {
        private Solution _donor;
        public DonorChange(Solution donor)
        {
            _donor = donor;
        }
        public Tuple<IList<string>, string> Apply(IterationData state, GOMVisualisationData visualisationData)
        {

            visualisationData.CurrentDonor = _donor;
            visualisationData.Solutions.ToList().ForEach(s =>  s.Selected = false );
            visualisationData.Solutions.First(d => d.Solution.Equals(_donor)).Selected = true;

            return new Tuple<IList<string>, string>(["CurrentDonor"], "Changed the donor to: \n" + _donor.Bits);
        }
    }
}
