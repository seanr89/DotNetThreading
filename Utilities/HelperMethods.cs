using System;
using System.Collections.Generic;

namespace Utilities
{
    /// <summary>
    /// Location to detail all basic and cross app functions
    /// </summary>
    public static class HelperMethods
    {
        /// <summary>
        /// Chunks the list
        /// </summary>
        /// <typeparam name="T">List object type</typeparam>
        /// <param name="list">List to be chunked</param>
        /// <param name="nSize">Size of a single chunk</param>
        /// <returns></returns>
        public static IEnumerable<List<T>> SplitList<T>(List<T> list, int nSize = 30)
        {
            for (int i = 0; i < list.Count; i += nSize)
            {
                yield return list.GetRange(i, Math.Min(nSize, list.Count - i));
            }
        }
    }
}