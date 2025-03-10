using System.Collections.Generic;

namespace Omnae.BusinessLayer.Util
{
    public static class ListEx
    {
        public static T GetOrDefault<T>(this IList<T> list, int index)
        {
            return ((index+1) > list.Count) ? default(T) : list[index];
        }
    }
}
