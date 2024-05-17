using DynamicData;
using DynamicData.Kernel;
using LiveChartsCore.Kernel;
using LLEAV.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.ViewModels.Controls.PopulationDepictions
{
    public abstract class PopulationContainerViewModelBase: ViewModelBase
    {
        public Population Population { get; set; }

        [Reactive]
        public bool Selected { get; set; }


        public PopulationContainerViewModelBase(Population population)
        {
            Population = population;
        }
    }

    public abstract class PopulationDepictionViewModelBase : ViewModelBase
    {
        public ObservableCollection<PopulationContainerViewModelBase> Containers { get; set; } = new ObservableCollection<PopulationContainerViewModelBase> { };
        public int SelectedPopulation { get; private set; } = -1;

        public abstract void Update(IterationData iteration);

        public void FindIndexOfSelected()
        {
            SelectedPopulation = Containers.Select((c, i) => (c, i)).FirstOrOptional(t => t.c.Selected == true).ValueOr(() => (null, -1)).i;
        }

        // Called when clicking on a population block
        public void SelectPopulation(PopulationContainerViewModelBase o)
        {
            if (!o.Selected)
            {
                ClearSelected();
                o.Selected = true;
            }
            GlobalManager.Instance.OpenPopulation(o.Population);
        }

        // Called when switching iterations
        public void SelectPopulation(int index)
        {
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
