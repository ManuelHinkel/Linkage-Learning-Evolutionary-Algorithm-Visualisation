using DynamicData.Kernel;
using LLEAV.Models;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Linq;

namespace LLEAV.ViewModels.Controls.PopulationDepictions
{
    /// <summary>
    /// Base view model for representing a population container.
    /// </summary>
    public abstract class PopulationContainerViewModelBase : ViewModelBase
    {
        /// <summary>
        /// Gets or sets the population associated with this container.
        /// </summary>
        public Population Population { get; set; }

        /// <summary>
        /// Gets or sets the selection state of the container.
        /// </summary>
        [Reactive]
        public bool Selected { get; set; }

        // <summary>
        /// Constructs an instance of PopulationContainerViewModelBase with the specified population.
        /// </summary>
        /// <param name="population">The population associated with this container.</param>
        public PopulationContainerViewModelBase(Population population)
        {
            Population = population;
        }
    }

    /// <summary>
    /// Base view model for managing a collection of population containers.
    /// </summary>
    public abstract class PopulationDepictionViewModelBase : ViewModelBase
    {
        /// <summary>
        /// Gets the collection of population containers.
        /// </summary>
        public ObservableCollection<PopulationContainerViewModelBase> Containers { get; set; } = new ObservableCollection<PopulationContainerViewModelBase> { };

        /// <summary>
        /// Gets or sets the index of the selected population container.
        /// </summary>
        public int SelectedPopulation { get; set; } = -1;

        /// <summary>
        /// Updates the view model with data from a new iteration.
        /// </summary>
        /// <param name="iteration">The iteration data to update the view model.</param>
        public abstract void Update(IterationData iteration);

        /// <summary>
        /// Selects a population container based on user interaction. Called, when clicking o a population container.
        /// </summary>
        /// <param name="o">The population container to select.</param>
        public void SelectPopulation(PopulationContainerViewModelBase o)
        {
            if (!o.Selected)
            {
                ClearSelected();
                o.Selected = true;
            }
            GlobalManager.Instance.OpenPopulation(o.Population);
            SelectedPopulation = Containers.Select((c, i) => (c, i)).FirstOrOptional(t => t.c.Selected == true).ValueOr(() => (null, -1)).i;
        }

        /// <summary>
        /// Selects a population container by index. Called when switching iterations
        /// </summary>
        /// <param name="index">The index of the population container to select.</param>
        public void SelectPopulation(int index)
        {
            SelectedPopulation = index;
            ClearSelected();
            if (index >= Containers.Count || index < 0) return;
            if (!Containers[index].Selected)
            {
                Containers[index].Selected = true;
                GlobalManager.Instance.UpdatePopulationWindowIfOpen(Containers[index].Population);
            }
        }

        private void ClearSelected()
        {
            foreach (PopulationContainerViewModelBase o in Containers)
            {
                o.Selected = false;
            }
        }
    }
}
