using LLEAV.Models;
using ReactiveUI;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace LLEAV.ViewModels.Controls.PopulationDepictions
{
    public class PopulationBlock : PopulationContainerViewModelBase
    {
        public PopulationBlock(Population population) : base(population)
        {
        }
    }

    public class PopulationBlocksViewModel : PopulationDepictionViewModelBase
    {
        public override void Update(IterationData iteration)
        {
            Containers.Clear();
            foreach (Population pop in iteration.Populations)
            {
                Containers.Add(new PopulationBlock(pop));
            }
            SelectPopulation(SelectedPopulation);
        }
    }
}
