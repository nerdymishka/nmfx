using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;

namespace NerdyMishka.Core
{
    public static class Check
    {
        public static T NotNull<T>([NotNullIfNotNull("value")] T? value, string parameterName)
        {
            if (value is null)
                throw new ArgumentNullException(parameterName);

            return value;
        }

        public static string NotNullOrEmpty([NotNullIfNotNull("value")] string? value, string parameterName)
        {
            if (IsNullOrEmpty(value))
                throw new ArgumentNullException(parameterName, $"{parameterName} must not be null or empty.");

            return value;
        }

        public static string NotNullOrWhiteSpace([NotNullIfNotNull("value")] string? value, string parameterName)
        {
            if (IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(parameterName, $"{parameterName} must not be null or empty.");

            return value;
        }

        public static bool IsNullOrEmpty([NotNullWhen(false)] string? value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNullOrWhiteSpace([NotNullWhen(false)] string? value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
    }
}
