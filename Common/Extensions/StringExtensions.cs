using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Web;

namespace Omnae.Common.Extensions
{
    public static class StringExtensions
    {
        private const string TimeSpanFormat = @"dd\:hh\:mm\:ss";

        public static int? ToInt(this string s)
        {
            return string.IsNullOrWhiteSpace(s) ? default : Convert.ToInt32(s);
        }

        public static decimal? ToDecimal(this string s)
        {
            return string.IsNullOrWhiteSpace(s) ? default : Convert.ToDecimal(s);
        }

        public static bool? ToBool(this string s)
        {
            return string.IsNullOrWhiteSpace(s) ? default : Convert.ToBoolean(s);
        }

        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        public static bool IsUrlEncoded(this string text)
        {
            return (HttpUtility.UrlDecode(text) != text);
        }

        public static T ConvertToType<T>(this object str)
        {
            if (typeof(T).IsEnum)
            {
                return (T)Enum.Parse(typeof(T), str.ToString());
            }
            if (typeof(T) == typeof(TimeSpan))
            {
                return (T)(object)TimeSpan.ParseExact(str.ToString(), TimeSpanFormat, CultureInfo.InvariantCulture);
            }
            if (str == null)
            {
                return default(T);
            }
            var t = typeof(T);
            return (T)Convert.ChangeType(str, Nullable.GetUnderlyingType(t) ?? t);
        }

        public static string SplitCamelCase(this string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return str;
            }
            return Regex.Replace(str, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }

        public static Guid ToGuid(this string str)
        {
            Guid result;
            return Guid.TryParse(str, out result) ? result : Guid.Empty;
        }

        public static T ToEnum<T>(this string str)
        {
            return (T) Enum.Parse(typeof (T), str);
        }

        public static string ToEncodedUrl(this string str)
        {
            return HttpUtility.UrlEncode(str) == str ? str : HttpUtility.UrlEncode(str);
        }

        private static readonly char[] HexDigits = "0123456789abcdef".ToCharArray();

        public static string ToHexString(this byte[] bytes)
        {
            var digits = new char[bytes.Length * 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                int d2;
                var d1 = Math.DivRem(bytes[i], 16, out d2);
                digits[2 * i] = HexDigits[d1];
                digits[2 * i + 1] = HexDigits[d2];
            }
            return new string(digits);
        }

        public static string ToCapital(this string str)
        {
            string ret = String.Empty;
            if(!String.IsNullOrEmpty(str))
            {
                if (str.Length > 1)
                    ret = Char.ToUpper(str[0]) + str.Substring(1);
                else 
                    ret = Char.ToUpper(str[0]).ToString();
            }
            return ret;
        }


        public static int? ExtractInt(this string str)
        {
            if (String.IsNullOrWhiteSpace(str))
                return null;

            var strWithoutLetters = Regex.Match(str, @"\d+").Value;

            var hasInteger = Int32.TryParse(strWithoutLetters, out int integer);

            if (!hasInteger)
                return null;

            return integer;
        }

        public static IEnumerable<string> SplitOnNewLines(this string source, StringSplitOptions option = StringSplitOptions.None)
        {
            return source.Split(new[] { "\r\n", "\r", "\n" }, option);
        }

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static bool IsNumeric(this string str)
        {
            return !String.IsNullOrWhiteSpace(str) && Single.TryParse(str, out _);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhiteSpace(this char ch)
        {
            // this is surprisingly faster than the equivalent if statement
            switch (ch)
            {
                case '\u0009':
                case '\u000A':
                case '\u000B':
                case '\u000C':
                case '\u000D':
                case '\u0020':
                case '\u0085':
                case '\u00A0':
                case '\u1680':
                case '\u2000':
                case '\u2001':
                case '\u2002':
                case '\u2003':
                case '\u2004':
                case '\u2005':
                case '\u2006':
                case '\u2007':
                case '\u2008':
                case '\u2009':
                case '\u200A':
                case '\u2028':
                case '\u2029':
                case '\u202F':
                case '\u205F':
                case '\u3000':
                    return true;
                default:
                    return false;
            }
        }

        public static string TrimInside(this string str)
        {
            while (str.Contains("  "))
            {
                str = str.Replace("  ", " ");
            }
            return str;
        }
        public static string TrimAll(this string str)
        {
            return str.TrimInside().Trim();
        }

        public static bool IsOneOf(this string str, params string[] listOfOptions)
        {
            return listOfOptions.Any(s => String.Equals(str, s));
        }

        public static string RemoveQueryStringFromUrl(this string url)
        {
            var newUri = new UriBuilder(url);
            newUri.Query = "";
            return newUri.ToString();
        }
        public static string RemoveQueryStringFromUrl(this string url, string key)
        {
            var uri = new Uri(url);

            // this gets all the query string key value pairs as a collection
            var newQueryString = HttpUtility.ParseQueryString(uri.Query);

            // this removes the key if exists
            newQueryString.Remove(key);

            // this gets the page path from root without QueryString
            string pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

            return newQueryString.Count > 0
                ? String.Format("{0}?{1}", pagePathWithoutQueryString, newQueryString)
                : pagePathWithoutQueryString;
        }

        public static string ToNullIfEmpty(this string src)
        {
            return string.IsNullOrWhiteSpace(src) ? null : src;
        }
    }
}
