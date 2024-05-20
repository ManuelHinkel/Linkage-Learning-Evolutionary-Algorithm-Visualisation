using LLEAV.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Algorithms.MIP.StateChange
{
    public class ParentsCreatedChange : IMIPStateChange
    {
        private IList<Tuple<Solution, Solution>> _generated;
        public ParentsCreatedChange(IList<Tuple<Solution, Solution>> generated) 
        {
            _generated = generated;
        }

        public Tuple<IList<string>,string> Apply(IterationData state, MIPVisualisationData visualisationData, bool onlyOperateOnData = false)
        {

            visualisationData.Solutions = _generated.Select(
                    s => new Tuple<SolutionWrapper, SolutionWrapper>(new SolutionWrapper(s.Item1), new SolutionWrapper(s.Item2))
                ).ToList();
            return new Tuple<IList<string>, string>(["Solutions"], "Created new solutions.");
        }

    }
}
