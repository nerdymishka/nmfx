using System;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle.Sdk
{
    public class SkippedTestMessageBus : IMessageBus
    {
        private readonly IMessageBus innerBus;

        public SkippedTestMessageBus(IMessageBus innerBus)
        {
            this.innerBus = innerBus;
        }

        public int SkippedTestCount { get; private set; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool QueueMessage(IMessageSinkMessage message)
        {
            if (message is ITestFailed testFailed)
            {
                foreach (var exceptionMessage in testFailed.Messages)
                {
                    if (!exceptionMessage.StartsWith(DynamicSkipToken.Value))
                        continue;

                    this.SkippedTestCount++;
                    if (exceptionMessage.Length > DynamicSkipToken.Value.Length)
                    {
                        var skipReason = exceptionMessage.Substring(DynamicSkipToken.Value.Length);
                        return this.innerBus.QueueMessage(new TestSkipped(testFailed.Test, skipReason));
                    }
                }
            }

            // Nothing we care about, send it on its way
            return this.innerBus.QueueMessage(message);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            this.innerBus?.Dispose();
        }
    }
}