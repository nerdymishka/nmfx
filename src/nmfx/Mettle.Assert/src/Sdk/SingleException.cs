using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Mettle.Sdk
{
    #pragma warning disable S3925

    [SuppressMessage("", "RCS1194", Justification = "Match XUnit APO")]
    public class SingleException : Xunit.Sdk.XunitException
    {
        public SingleException(string errorMessage)
              : base(errorMessage)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="SingleException"/> for when the collection didn't contain any of the expected value.
        /// </summary>
        /// <param name="expected">The expected argument.</param>
        /// <returns>An a single excpetion.</returns>
        public static Exception Empty([AllowNull] string? expected) =>
            new SingleException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "The collection was expected to contain a single element{0}, but it {1}",
                    expected == null ? string.Empty : " matching " + expected,
                    expected == null ? "was empty." : "contained no matching elements."));

        /// <summary>
        /// Creates an instance of <see cref="SingleException"/> for when the collection had too many of the expected items.
        /// </summary>
        /// <param name="count">The expected count.</param>
        /// <param name="expected">The expected argument.</param>
        /// <returns>An a single excpetion.</returns>
        public static Exception MoreThanOne(int count, [AllowNull] string? expected) =>
            new SingleException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "The collection was expected to contain a single element{0}, but it contained {1}{2} elements.",
                    expected == null ? string.Empty : " matching " + expected,
                    count,
                    expected == null ? string.Empty : " matching"));
    }
}