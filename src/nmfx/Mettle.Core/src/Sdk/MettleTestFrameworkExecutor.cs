using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle.Sdk
{
    public class MettleTestFrameworkExecutor : XunitTestFrameworkExecutor
    {
        public MettleTestFrameworkExecutor(
            AssemblyName assemblyName,
            ISourceInformationProvider sourceInformationProvider,
            IMessageSink diagnosticMessageSink)
            : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
        {
        }

        /// <inheritdoc />
        [SuppressMessage("AsyncUsage", "AsyncFixer03:Fire-and-forget async-void methods or delegates", Justification = "Required by xunit api")]
        protected override async void RunTestCases(
            IEnumerable<IXunitTestCase> testCases,
            IMessageSink executionMessageSink,
            ITestFrameworkExecutionOptions executionOptions)
        {
            IServiceProvider? serviceProvider = null;
            var exceptionList = new List<Exception>();
            try
            {
                serviceProvider = MettleServiceProviderLocator.GetServiceProvider(this.TestAssembly.Assembly);
            }
            catch (Exception ex)
            {
                exceptionList.Add(ex);
            }

            using var runner = new MettleTestAssemblyRunner(
                serviceProvider,
                this.TestAssembly,
                testCases,
                this.DiagnosticMessageSink,
                executionMessageSink,
                executionOptions,
                exceptionList);

            await runner.RunAsync().ConfigureAwait(false);
        }
    }
}