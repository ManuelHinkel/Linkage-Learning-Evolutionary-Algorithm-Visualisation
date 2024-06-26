﻿using LLEAV.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.ROM.StateChange
{
    public class TournamentSelectionChange : IROMStateChange
    {
        private IList<Solution> _solutions;
        
        public TournamentSelectionChange(IList<Solution> solutions)
        { 
            _solutions = solutions; 
        }

        public Tuple<IList<string>, Message> Apply(IterationData state, ROMVisualisationData visualisationData, bool onlyOperateOnData = false)
        {

            visualisationData.NextIteration = new ObservableCollection<SolutionWrapper> (_solutions.Select(s => new SolutionWrapper(s)).ToList());

            return new Tuple<IList<string>, Message>(["NextIteration"], 
                new Message("Applied tournament Selection", MessagePriority.IMPORTANT));
        }
    }
}
