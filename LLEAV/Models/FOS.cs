using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models
{
    public class FOS : IEnumerable<Cluster>
    {
        public FOSType Type { get; set; }
        public IList<Cluster> Clusters { get; }

        public FOS(IList<Cluster> clusters, FOSType fosType)
        {
            Clusters = clusters;
            Type = fosType;
        }

        public IEnumerator<Cluster> GetEnumerator()
        {
            return Clusters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public enum FOSType
    {
        UNIVARIATE,
        MARGINAL_PRODUCT,
        LINKAGE_TREE,
    }
}
