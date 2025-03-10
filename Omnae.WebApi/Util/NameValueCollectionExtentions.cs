using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.Util
{
    public static class NameValueCollectionExtentions
    {
        public static IDictionary<string, string> ToDictionary(this NameValueCollection nvc)
        {
            return nvc.AllKeys.ToDictionary(k => k, k => nvc[k]);
        }
        public static IDictionary<string, object> ToDictionaryOfObjects(this NameValueCollection nvc)
        {
            return nvc.AllKeys.ToDictionary(k => k, k => (object) nvc[k]);
        }

        public static IDictionary<string, object> ClearEmptyEntries(this IDictionary<string, object> dic)
        {
            foreach (var entry in dic)
            {
                if (entry.Value == null)
                {
                    dic.Remove(entry);
                }
            }
            return dic;
        }

        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this NameValueCollection col)
        {
            var dict = new Dictionary<TKey, TValue>();
            var keyConverter = TypeDescriptor.GetConverter(typeof(TKey));
            var valueConverter = TypeDescriptor.GetConverter(typeof(TValue));

            foreach (string name in col)
            {
                TKey key = (TKey)keyConverter.ConvertFromString(name);
                TValue value = (TValue)valueConverter.ConvertFromString(col[name]);
                dict.Add(key, value);
            }

            return dict;
        }
    }
}