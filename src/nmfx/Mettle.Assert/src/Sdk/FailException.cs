using Xunit.Sdk;

namespace Mettle.Sdk
{
    #pragma warning disable S3925, RCS1194

    public class FailException : XunitException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailException" /> class.
        /// </summary>
        /// <param name="message">The user's failure message.</param>
        public FailException(string message)
            : base($"Assert.Fail(): {message}")
        {
        }
    }
}