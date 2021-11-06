using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xunit.Sdk;

namespace Mettle.Sdk
{
#pragma warning disable SA1101, SA1400, SA1314, SA1519, RCS1194, S3925

    /// <summary>
    /// Exception thrown when two values are unexpectedly not equal.
    /// </summary>
    public class EqualException : AssertActualExpectedException
    {
        private static readonly Dictionary<char, string> Encodings = new()
        {
            { '\r', "\\r" },
            { '\n', "\\n" },
            { '\t', "\\t" },
            { '\0', "\\0" },
        };

        private string? message;

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualException"/> class.
        /// </summary>
        /// <param name="expected">The expected object value.</param>
        /// <param name="actual">The actual object value.</param>
        public EqualException(object? expected, object? actual)
            : base(expected, actual, "Assert.Equal() Failure")
        {
            ActualIndex = -1;
            ExpectedIndex = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualException"/> class for string comparisons.
        /// </summary>
        /// <param name="expected">The expected string value.</param>
        /// <param name="actual">The actual string value.</param>
        /// <param name="expectedIndex">The first index in the expected string where the strings differ.</param>
        /// <param name="actualIndex">The first index in the actual string where the strings differ.</param>
        public EqualException(string? expected, string? actual, int expectedIndex, int actualIndex)
            : this(expected, actual, expectedIndex, actualIndex, null)
        {
        }

        public EqualException(string? expected, string? actual, int expectedIndex, int actualIndex, int? pointerPosition)

            : base(expected, actual, "Assert.Equal() Failure")
        {
            ActualIndex = actualIndex;
            ExpectedIndex = expectedIndex;
            PointerPosition = pointerPosition;
        }

        /// <summary>
        /// Gets the index into the actual value where the values first differed.
        /// Returns -1 if the difference index points were not provided.
        /// </summary>
        public int ActualIndex { get; }

        /// <summary>
        /// Gets the index into the expected value where the values first differed.
        /// Returns -1 if the difference index points were not provided.
        /// </summary>
        public int ExpectedIndex { get; }

        /// <inheritdoc/>
        public override string Message
        {
            get { return this.message ??= this.CreateMessage(); }
        }

        /// <summary>
        /// Gets the index of the difference between the IEnumerables when converted to a string.
        /// </summary>
        public int? PointerPosition { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="EqualException"/> class for IEnumerable comparisons.
        /// </summary>
        /// <param name="expected">The expected object value.</param>
        /// <param name="actual">The actual object value.</param>
        /// <param name="mismatchIndex">The first index in the expected IEnumerable where the strings differ.</param>
        /// <returns>A EqualException.</returns>
        public static EqualException FromEnumerable(IEnumerable? expected, IEnumerable? actual, int mismatchIndex)
        {
            var expectedText = ArgumentFormatter.Format(expected, out var pointerPositionExpected, mismatchIndex);
            var actualText = ArgumentFormatter.Format(actual, out var pointerPositionActual, mismatchIndex);
            var pointerPosition = (pointerPositionExpected ?? -1) > (pointerPositionActual ?? -1) ? pointerPositionExpected : pointerPositionActual;

            return new EqualException(expectedText, actualText, mismatchIndex, mismatchIndex, pointerPosition);
        }

        private static Tuple<string, string> ShortenAndEncode(string? value, int position, char pointer, int? index = null)
        {
            if (value == null)
                return Tuple.Create("(null)", string.Empty);

            index ??= position;

            var start = Math.Max(position - 20, 0);
            var end = Math.Min(position + 41, value.Length);
            var printedValue = new StringBuilder(100);
            var printedPointer = new StringBuilder(100);

            if (start > 0)
            {
                printedValue.Append("···");
                printedPointer.Append("   ");
            }

            for (var idx = start; idx < end; ++idx)
            {
                var c = value[idx];
                var paddingLength = 1;

                if (Encodings.TryGetValue(c, out var encoding))
                {
                    printedValue.Append(encoding);
                    paddingLength = encoding.Length;
                }
                else
                {
                    printedValue.Append(c);
                }

                if (idx < position)
                    printedPointer.Append(' ', paddingLength);
                else if (idx == position)
                    printedPointer.AppendFormat("{0} (pos {1})", pointer, index);
            }

            if (value.Length == position)
                printedPointer.AppendFormat("{0} (pos {1})", pointer, index);

            if (end < value.Length)
                printedValue.Append("···");

            return Tuple.Create(printedValue.ToString(), printedPointer.ToString());
        }

        private string CreateMessage()
        {
            if (ExpectedIndex == -1)
                return base.Message;

            var printedExpected = ShortenAndEncode(Expected, PointerPosition ?? ExpectedIndex, '↓', ExpectedIndex);
            var printedActual = ShortenAndEncode(Actual, PointerPosition ?? ActualIndex, '↑', ActualIndex);

            var sb = new StringBuilder();
            sb.Append(UserMessage);

            if (!string.IsNullOrWhiteSpace(printedExpected.Item2))
            {
                sb.AppendFormat(
                    CultureInfo.CurrentCulture,
                    "{0}          {1}",
                    Environment.NewLine,
                    printedExpected.Item2);
            }

            sb.AppendFormat(
                CultureInfo.CurrentCulture,
                "{0}Expected: {1}{0}Actual:   {2}",
                Environment.NewLine,
                printedExpected.Item1,
                printedActual.Item1);

            if (!string.IsNullOrWhiteSpace(printedActual.Item2))
            {
                sb.AppendFormat(
                    CultureInfo.CurrentCulture,
                    "{0}          {1}",
                    Environment.NewLine,
                    printedActual.Item2);
            }

            return sb.ToString();
        }
    }
}