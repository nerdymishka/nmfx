using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle.Sdk
{
    /// <summary>Wraps another test case that should be skipped.</summary>
    public class MettleTestCase : XunitTestCase
    {
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

            this.serviceProvider = serviceProvider;

            var serviceProviderOverride = MettleServiceProviderLocator.GetServiceProvider(this.TestMethod);

            if (serviceProviderOverride != null)
            {
                this.serviceProvider = serviceProviderOverride;
            }
            else if (this.serviceProvider == null)
            {
                serviceProviderOverride = MettleServiceProviderLocator.GetServiceProvider(this.TestMethod.TestClass);
                if (serviceProviderOverride != null)
                    this.serviceProvider = serviceProviderOverride;
            }

            if (traits is null)
                return;

            foreach (var kvp in traits)
            {
                this.Traits.Add(kvp.Key, kvp.Value);
            }

            this.ServiceProvider = this.serviceProvider;
        }

        protected IServiceProvider? ServiceProvider { get; }

        public override async Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        {
            var messageBusInterceptor = new SkippedTestMessageBus(messageBus);
            using var runner = new MettleTestCaseRunner(
                this.serviceProvider,
                this,
                this.DisplayName,
                this.SkipReason,
                constructorArguments,
                this.TestMethodArguments,
                messageBus,
                aggregator,
                cancellationTokenSource);

            var result = await runner
                .RunAsync()
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