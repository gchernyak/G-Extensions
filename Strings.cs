using System;
using System.Threading;

namespace G.Extensions
{
    public static class Strings
    {
        public static bool IsAllUpper(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (!Char.IsUpper(s[i]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Use the current thread's culture info for conversion
        /// </summary>
        public static string ToTitleCase(this string str)
        {
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
        }

        public static T ToEnum<T>(this string value, T defaultValue) where T : struct
        {
            if (String.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            T result;
            return Enum.TryParse<T>(value, true, out result) ? result : defaultValue;
        }

        public static string StringDataType(this string value)
        {
            var clean = value.Trim();
            if (clean.StartsWith("{") || clean.StartsWith("["))
            {
                return "json";
            }
            if (clean.StartsWith("<"))
            {
                return "xml";
            }
            if (clean.Contains(",") || clean.Contains("="))
            {
                return "list";
            }
            return null;
        }
    }
}