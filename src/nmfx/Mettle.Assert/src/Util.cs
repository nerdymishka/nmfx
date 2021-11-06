using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace Mettle
{
    internal static class Util
    {
        internal const string AssemblyName = "NerdyMishka.Mettle";

        internal const string SdkNamespace = "Mettle.Sdk";

        internal const string RootNamespace = "Mettle";

        internal const string TestFrameworkDiscoverer = SdkNamespace + ".MettleTestFrameworkTypeDiscoverer";

        internal const string TestCaseDiscoverer = SdkNamespace + ".TestCaseDiscoverer";

        public static bool IsLineEnding(char c) =>
            c == '\r' || c == '\n';

        public static bool IsWhiteSpace(char c)
        {
            const char mongolianVowelSeparator = '\u180E';
            const char zeroWidthSpace = '\u200B';
            const char zeroWidthNoBreakSpace = '\uFEFF';
            const char tabulation = '\u0009';

            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);

            return
                unicodeCategory == UnicodeCategory.SpaceSeparator ||
                c == mongolianVowelSeparator ||
                c == zeroWidthSpace ||
                c == zeroWidthNoBreakSpace ||
                c == tabulation;
        }

        public static int SkipLineEnding(ReadOnlySpan<char> value, int index)
        {
            if (value[index] == '\r')
                ++index;

            if (index < value.Length && value[index] == '\n')
                ++index;

            return index;
        }

        public static int SkipWhitespace(ReadOnlySpan<char> value, int index)
        {
            while (index < value.Length)
            {
                if (IsWhiteSpace(value[index]))
                    index++;
                else
                    return index;
            }

            return index;
        }

        public static T? GetSingleResult<T>(
            IEnumerable<T> collection,
            [AllowNull] Predicate<T> predicate,
            [AllowNull] string expectedArgument)
        {
            var count = 0;
            var result = default(T);

            foreach (var item in collection)
            {
                if ((predicate == null || predicate(item)) && ++count == 1)
                    result = item;
            }

            switch (count)
            {
                case 0:
                    throw Mettle.Sdk.SingleException.Empty(expectedArgument);
                case 1:
                    return result;
                default:
                    throw Mettle.Sdk.SingleException.MoreThanOne(count, expectedArgument);
            }
        }

        /// <summary>
        /// Records any exception which is thrown by the given code.
        /// </summary>
        /// <param name="testCode">The code which may thrown an exception.</param>
        /// <returns>Returns the exception that was thrown by the code; null, otherwise.</returns>
        public static Exception? RecordException(Action testCode)
        {
            if (testCode is null)
                throw new ArgumentNullException(nameof(testCode));

            try
            {
                testCode();
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// Records any exception which is thrown by the given code that has
        /// a return value. Generally used for testing property accessors.
        /// </summary>
        /// <param name="testCode">The code which may thrown an exception.</param>
        /// <returns>Returns the exception that was thrown by the code; null, otherwise.</returns>
        public static Exception? RecordException([AllowNull] Func<object> testCode)
        {
            if (testCode is null)
                throw new ArgumentNullException(nameof(testCode));

            Task? task = null;

            try
            {
                task = testCode() as Task;
            }
            catch (Exception ex)
            {
                return ex;
            }

            if (task != null)
                throw new InvalidOperationException("You must call Assert.ThrowsAsync, Assert.DoesNotThrowAsync, or Record.ExceptionAsync when testing async code.");

            return null;
        }

        /// <summary>
        /// Records any exception which is thrown by the given task.
        /// </summary>
        /// <param name="testCode">The task which may thrown an exception.</param>
        /// <returns>Returns the exception that was thrown by the code; null, otherwise.</returns>
        [SuppressMessage("", "S4457", Justification = "Follows Xunit")]
        public static async Task<Exception?> RecordExceptionAsync(Func<Task> testCode)
        {
            if (testCode is null)
                throw new ArgumentNullException(nameof(testCode));

            try
            {
                await testCode().ConfigureAwait(false);
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

#if NETSTANDARD2_1
        /// <summary>
        /// Records any exception which is thrown by the given task.
        /// </summary>
        /// <param name="testCode">The task which may thrown an exception.</param>
        /// <returns>Returns the exception that was thrown by the code; null, otherwise.</returns>
        [SuppressMessage("", "S4457", Justification = "Follows Xunit")]
        public static async ValueTask<Exception?> RecordExceptionAsync(Func<ValueTask> testCode)
        {
            if (testCode is null)
                throw new ArgumentNullException(nameof(testCode));

            try
            {
                await testCode().ConfigureAwait(false);
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// Records any exception which is thrown by the given task.
        /// </summary>
        /// <param name="testCode">The task which may thrown an exception.</param>
        /// <typeparam name="T">The type of the ValueTask return value.</typeparam>
        /// <returns>Returns the exception that was thrown by the code; null, otherwise.</returns>
        [SuppressMessage("", "S4457", Justification = "Follows Xunit")]
        public static async ValueTask<Exception?> RecordExceptionAsync<T>(Func<ValueTask<T>> testCode)
        {
            if (testCode is null)
                throw new ArgumentNullException(nameof(testCode));

            try
            {
                await testCode().ConfigureAwait(false);
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

#endif

        public static Exception Throws(Type exceptionType, [AllowNull] Exception exception)
        {
            if (exceptionType is null)
                throw new ArgumentNullException(nameof(exceptionType));

            if (exception == null)
                throw new ThrowsException(exceptionType);

            if (!exceptionType.Equals(exception.GetType()))
                throw new ThrowsException(exceptionType, exception);

            return exception;
        }

        public static Exception ThrowsAny(Type exceptionType, [AllowNull] Exception exception)
        {
            if (exceptionType is null)
                throw new ArgumentNullException(nameof(exceptionType));

            if (exception == null)
                throw new ThrowsException(exceptionType);

            if (!exceptionType.GetTypeInfo().IsAssignableFrom(exception.GetType().GetTypeInfo()))
                throw new ThrowsException(exceptionType, exception);

            return exception;
        }
    }
}