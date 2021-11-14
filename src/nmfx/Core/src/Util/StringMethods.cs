using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using NerdyMishka.Collections.Generic;

namespace NerdyMishka.Util
{
    public static class StringMethods
    {
        public static bool EqualsCultureCaseInsensitive([NotNull] this string left, string? right)
        {
            return left.Equals(right, StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool EqualsCaseInsensitive([NotNull] this string left, string? right)
        {
            return left.Equals(right, StringComparison.OrdinalIgnoreCase);
        }

        public static IEnumerable<string> ToLinesEnumerable(this string? value)
        {
            if (value == null)
                return Array.Empty<string>();

            return new LinesEnumerable(value);
        }

        public static List<string> ToLines(this string value)
        {
            return new List<string>(ToLinesEnumerable(value));
        }
    }
}
