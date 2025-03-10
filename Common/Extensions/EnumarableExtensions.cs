using System.Collections.Generic;
using System.Linq;

namespace Common.Extensions
{
    public static class EnumarableExtensions
    {
        public static ICollection<T> ToNullIfEmpty<T>(this ICollection<T> list)
        {
            if (list is null)
                return null;

            return list.Count == 0 ? null : list;
        }
        public static IEnumerable<T> ToNullIfEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable is null)
                return null;

            if (enumerable is ICollection<T> c)
                return ToNullIfEmpty(c);

            return enumerable.Any() == false ? null : enumerable;
        }
    }
}
