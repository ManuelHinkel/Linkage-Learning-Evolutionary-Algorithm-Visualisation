﻿using LLEAV.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.ROM.StateChange
{
    public class FOSChange: IROMStateChange
    {

        private Population _population;
        public FOSChange(Population population)
        {
            _population = population;
        }
        public Tuple<IList<string>, string> Apply(IterationData state, ROMVisualisationData visualisationData)
        {
           
            state.Populations[_population.PyramidIndex] = _population;

            return new Tuple<IList<string>, string>([], "Changed FOS of population.");
        }

    }
}