using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Generic
{
    public class CollectionExtensions
    {
    }
    public static class ListExtensions
    {
        public static bool HaveSameElements<T>(this List<T> list, List<T> other)
            where T : IEquatable<T>
        {
            if (list.Except(other).Any())
                return false;
            if (other.Except(list).Any())
                return false;
            return true;
        }


        public static IEnumerable<TSource> Exclude<TSource, TKey>(this IEnumerable<TSource> source,
            IEnumerable<TSource> exclude, Func<TSource, TKey> keySelector)
        {

            var excludedSet = new HashSet<TKey>(exclude.Select(keySelector));
            return source.Where(item => !excludedSet.Contains(keySelector(item)));
        }
    }
}