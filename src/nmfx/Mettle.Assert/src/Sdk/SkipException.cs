using Xunit.Sdk;

namespace Mettle.Sdk
{
    #pragma warning disable S3925, RCS1194

    /// <summary>
    /// Exception thrown when a test should be skipped.
    /// </summary>
    public class SkipException : XunitException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkipException" /> class. This is a special
        /// exception that, when thrown, will cause xUnit.net to mark your test as skipped
        /// rather than failed.
        /// </summary>
        /// <param name="message">The reason for skipping a test.</param>
        public SkipException(string message)
            : base($"{DynamicSkipToken.Value}{message}")
        {
        }
    }
}