using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Omnae.Common;
using Humanizer;

namespace Omnae.WebApi.Util
{
    /// <summary>
    /// Class for manipulation of Enum type
    /// </summary>
    public static class EnumExtentions
    {
        /// <summary>
        /// Convert a Enum to a dictionary of int and string type
        /// </summary>
        public static Dictionary<int, string> ToDictionary<T>()
            where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("Type must be an enum");

            var dic = Enum.GetValues(typeof(T))
                          .Cast<Enum>()
                          .ToDictionary(t => (int)(object) t, t => t.Humanize());
            return dic;
        }
    }
}