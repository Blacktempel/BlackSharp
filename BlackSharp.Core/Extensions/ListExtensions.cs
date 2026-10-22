// SPDX-License-Identifier: MPL-2.0

namespace BlackSharp.Core.Extensions
{
    /// <summary>
    /// Extension class for <see cref="List{T}"/>.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Removes all elements which match specified predicate.
        /// </summary>
        /// <param name="list">This object.</param>
        /// <param name="match">Predicate to check if element should be removed.</param>
        public static void RemoveIf<T>(this List<T> list, Predicate<T> match)
        {
            for (int i = 0; i < list.Count;)
            {
                var item = list[i];
                if (match(item))
                    list.Remove(item);
                else
                    ++i;
            }
        }
    }
}
