using System;

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
    }
}
