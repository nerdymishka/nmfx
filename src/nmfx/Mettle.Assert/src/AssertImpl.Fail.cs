using System;
using Mettle.Sdk;

namespace Mettle
{
    public partial class AssertImpl
    {
        public void Fail(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));

            throw new FailException(message);
        }
    }
}