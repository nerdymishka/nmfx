using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle.Sdk
{
    /// <summary>Wraps another test case that should be skipped.</summary>
    public sealed class MettleTestCase : XunitTestCase
    {
#pragma warning disable CS0169, RCS1213, RCS1163, S1144, IDE0051, IDE0060
        private readonly IServiceProvider? serviceProvider;

        private string? skipReason;

        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public MettleTestCase()
            : base()
        {
        }

        public MettleTestCase(
            string? skipReason,
            IEnumerable<KeyValuePair<string, List<string?>>>? traits,
            IServiceProvider? serviceProvider,
            IMessageSink diagnosticMessageSink,
            TestMethodDisplay defaultMethodDisplay,
            TestMethodDisplayOptions defaultMethodDisplayOptions,
            ITestMethod testMethod,
            object[]? testMethodArguments = null)
            : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod, testMethodArguments)
        {
            this.skipReason = skipReason;

            if (traits is null)
                return;

            foreach (var kvp in traits)
            {
                this.Traits.Add(kvp.Key, kvp.Value);
            }
        }

        public override async Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        {
            var messageBusInterceptor = new SkippedTestMessageBus(messageBus);
            RunSummary? result = await base.RunAsync(
                    diagnosticMessageSink,
                    messageBusInterceptor,
                    constructorArguments,
                    aggregator,
                    cancellationTokenSource)
                .ConfigureAwait(false);

            result.Failed -= messageBusInterceptor.SkippedTestCount;
            result.Skipped += messageBusInterceptor.SkippedTestCount;

            return result;
        }

        public override void Deserialize(IXunitSerializationInfo data)
        {
            base.Deserialize(data);
            this.skipReason = data.GetValue<string>(nameof(this.skipReason));
        }

        public override void Serialize(IXunitSerializationInfo data)
        {
            base.Serialize(data);
            data.AddValue(nameof(this.skipReason), this.skipReason);
        }

        protected override string GetSkipReason(IAttributeInfo factAttribute)
            => this.skipReason ?? base.GetSkipReason(factAttribute);
    }
}