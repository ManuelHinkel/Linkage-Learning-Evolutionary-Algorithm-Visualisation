﻿using LLEAV.Models;
using LLEAV.ViewModels;
using LLEAV.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.MIP.StateChange
{
    public class DonorChange : IMIPStateChange
    {
        private Solution _donor;
        public DonorChange(Solution donor) 
        { 
            _donor = donor;
        }
        public Tuple<IList<string>, Message> Apply(IterationData state, MIPVisualisationData visualisationData, bool onlyOperateOnData = false)
        {

            visualisationData.CurrentDonor = _donor;

            visualisationData.Donors.ToList().ForEach(s => s.Selected = false);
            visualisationData.Donors.First(d => d.Solution.Equals(_donor)).Selected = true;

            return new Tuple<IList<string>, Message>(["CurrentDonor"], 
                new Message("Changed the donor to: \n" + _donor.Bits));
        }

    }
}
