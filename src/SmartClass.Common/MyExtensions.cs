using System;
using System.Collections.Generic;

namespace SmartClass.Common
{
    public static partial class MyExtensions
    {
        public static bool MyEquals(this string value, string value2, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var valueFix = string.Empty;
            if (!string.IsNullOrWhiteSpace(value))
            {
                valueFix = value.Trim();
            }

            var value2Fix = string.Empty;
            if (!string.IsNullOrWhiteSpace(value2))
            {
                value2Fix = value2.Trim();
            }

            return valueFix.Equals(value2Fix, comparison);
        }

        public static bool MyContains(this IEnumerable<string> values, string toCheck, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            foreach (var value in values)
            {
                if (value.MyEquals(toCheck, comparison))
                {
                    return true;
                }
            }
            return false;
        }

        public static string MyFind(this IEnumerable<string> values, string toCheck, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            foreach (var value in values)
            {
                if (value.MyEquals(toCheck, comparison))
                {
                    return value;
                }
            }
            return null;
        }
    }
}
