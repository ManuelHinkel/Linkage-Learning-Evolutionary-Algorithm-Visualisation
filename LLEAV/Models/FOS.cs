using System.Collections;
using System.Collections.Generic;

namespace LLEAV.Models
{

    /// <summary>
    /// Represents a Family of Subsets (FOS), which is a collection of clusters.
    /// </summary>
    public class FOS : IEnumerable<Cluster>
    {
        /// <summary>
        /// Gets or sets the type of the FOS.
        /// </summary>
        public FOSType Type { get; set; }

        /// <summary>
        /// Gets the list of clusters in the FOS.
        /// </summary>
        public IList<Cluster> Clusters { get; }


        /// <summary>
        /// Initializes a new instance of the FOS with the specified clusters and FOS type.
        /// </summary>
        /// <param name="clusters">The list of clusters in the FOS.</param>
        /// <param name="fosType">The type of the FOS.</param>
        public FOS(IList<Cluster> clusters, FOSType fosType)
        {
            Clusters = clusters;
            Type = fosType;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the clusters in the FOS.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the clusters in the FOS.</returns>
        public IEnumerator<Cluster> GetEnumerator()
        {
            return Clusters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// Enumeration for the different types of FOS.
    /// </summary>
    public enum FOSType
    {
        UNIVARIATE,
        MARGINAL_PRODUCT,
        LINKAGE_TREE,
    }
}
