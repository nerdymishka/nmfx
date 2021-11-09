using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle.Sdk
{
    public class MettleTheoryTestCase : MettleTestCase
    {
        [Obsolete(
            "Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public MettleTheoryTestCase()
            : base()
        {
        }

        public MettleTheoryTestCase(
            string? skipReason,
            IEnumerable<KeyValuePair<string, List<string?>>>? traits,
            IServiceProvider? serviceProvider,
            IMessageSink diagnosticMessageSink,
            TestMethodDisplay defaultMethodDisplay,
            TestMethodDisplayOptions defaultMethodDisplayOptions,
            ITestMethod testMethod,
            object[]? testMethodArguments = null)
            : base(
                skipReason,
                traits,
                serviceProvider,
                diagnosticMessageSink,
                defaultMethodDisplay,
                defaultMethodDisplayOptions,
                testMethod,
                testMethodArguments)
        {
        }
    }
}
