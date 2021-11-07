using System;
using System.Diagnostics.CodeAnalysis;
using SkipException = Mettle.Sdk.SkipException;

namespace Mettle
{
    public partial class AssertImpl
    {
        /// <summary>
        /// Skips the current test. Used when determining whether a test should be skipped
        /// happens at runtime rather than at discovery time.
        /// </summary>
        /// <param name="reason">The message to indicate why the test was skipped.</param>
        /// <exception cref="SkipException">Thrown when the method is invoked.</exception>
        /// <exception cref="ArgumentNullException">Thrown the string is null or empty.</exception>
        [DoesNotReturn]
        public void Skip(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentNullException(nameof(reason));

            throw new SkipException(reason);
        }

        /// <summary>
        /// Will skip the current test unless <paramref name="condition"/> evaluates to <c>true</c>.
        /// </summary>
        /// <param name="condition">When <c>true</c>, the test will continue to run; when <c>false</c>,
        /// the test will be skipped.</param>
        /// <param name="reason">The message to indicate why the test was skipped.</param>
        /// <returns>An instance of <see cref="IAssert" />.</returns>
        /// <exception cref="SkipException">Thrown when the condition is not met.</exception>
        /// <exception cref="ArgumentNullException">Thrown the string is null or empty.</exception>
        public IAssert SkipUnless([DoesNotReturnIf(false)] bool condition, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentNullException(nameof(reason));

            if (!condition)
                throw new SkipException(reason);

            return this;
        }

        /// <summary>
        /// Will skip the current test when <paramref name="condition"/> evaluates to <c>true</c>.
        /// </summary>
        /// <param name="condition">When <c>true</c>, the test will be skipped; when <c>false</c>,
        /// the test will continue to run.</param>
        /// <param name="reason">The message to indicate why the test was skipped.</param>
        /// <returns>An instance of <see cref="IAssert" />.</returns>
        /// <exception cref="SkipException">Thrown when the condition is met.</exception>
        /// <exception cref="ArgumentNullException">Thrown the string is null or empty.</exception>
        public IAssert SkipWhen([DoesNotReturnIf(true)] bool condition, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentNullException(nameof(reason));

            if (condition)
                throw new SkipException(reason);

            return this;
        }
    }
}