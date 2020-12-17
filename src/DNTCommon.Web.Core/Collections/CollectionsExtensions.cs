using System;
using System.Collections.Generic;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Collections Extensions
    /// </summary>
    public static class CollectionsExtensions
    {
        /// <summary>
        /// Adds the `AddRange` to an `IList`
        /// </summary>
        public static void AddRange<T>(this IList<T> source, IEnumerable<T> newList)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (newList == null)
            {
                throw new ArgumentNullException(nameof(newList));
            }

            if (source is List<T> concreteList)
            {
                concreteList.AddRange(newList);
                return;
            }

            foreach (var element in newList)
            {
                source.Add(element);
            }
        }
    }
}