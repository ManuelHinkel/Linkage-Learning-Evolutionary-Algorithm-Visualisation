using LLEAV.Models;
using ReactiveUI;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace LLEAV.ViewModels.Controls.PopulationDepictions
{
    /// <summary>
    /// Represents the block depiction of a population
    /// </summary>
    public class PopulationBlock : PopulationContainerViewModelBase
    {
        /// <summary>
        /// Constructs a new instance of PopulationBlock.
        /// </summary>
        /// <param name="population">The population assigned to the container.</param>
        public PopulationBlock(Population population) : base(population)
        {
        }
    }

    /// <summary>
    /// ViewModel for managing multiple population blocks based on provided run data.
    /// </summary>
    public class PopulationBlocksViewModel : PopulationDepictionViewModelBase
    {
        /// <summary>
        /// Updates the ViewModel with data from a new iteration.
        /// </summary>
        /// <param name="iteration">Iteration data to visualize.</param>
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
