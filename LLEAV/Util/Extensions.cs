using System;
using System.Collections.Generic;

namespace LLEAV.Util
{
    /// <summary>
    /// Utility class which handles extensions needed.
    /// </summary>
    static class Extensions
    {
        /// <summary>
        /// Shuffles a list randomly with a given random number generator.
        /// </summary>
        /// <typeparam name="T">The type of objects in the list.</typeparam>
        /// <param name="list">The list to be shuffled.</param>
        /// <param name="random">The random number generator used to shuffle the list.</param>
        public static void Shuffle<T>(this IList<T> list, Random random)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
