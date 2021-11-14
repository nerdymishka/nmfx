using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Util
{
    public static class EnumerableMethods
    {
        public static IEnumerable<T> FromParams<T>(params T[]? values)
        {
            return values ?? Array.Empty<T>();
        }
    }
}
