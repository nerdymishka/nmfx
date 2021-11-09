using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle.Sdk
{
    /// <summary>
    /// Mettle test method runner for xunit v2 tests. The test method runner creates
    /// the test case runner and invokes it.
    /// </summary>
    public class MettleTestMethodRunner : TestMethodRunner<IXunitTestCase>
    {
        private readonly IServiceProvider? serviceProvider;
        private readonly IMessageSink diagnosticMessageSink;
        private readonly object?[] constructorArguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="MettleTestMethodRunner"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider for injecting dependencies.</param>
        /// <param name="testMethod">The test method to be run.</param>
        /// <param name="class">The test class that contains the test method.</param>
        /// <param name="method">The test method that contains the tests to be run.</param>
        /// <param name="testCases">The test cases to be run.</param>
        /// <param name="diagnosticMessageSink">The message sink used to send diagnostic messages to.</param>
        /// <param name="messageBus">The message bus to report run status to.</param>
        /// <param name="aggregator">The exception aggregator used to run code and collect exceptions.</param>
        /// <param name="cancellationTokenSource">The task cancellation token source, used to cancel the test run.</param>
        /// <param name="constructorArguments">The constructor arguments for the test class.</param>
        public MettleTestMethodRunner(
            IServiceProvider? serviceProvider,
            ITestMethod testMethod,
            IReflectionTypeInfo @class,
            IReflectionMethodInfo method,
            IEnumerable<IXunitTestCase> testCases,
            IMessageSink diagnosticMessageSink,
            IMessageBus messageBus,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource,
            object?[] constructorArguments)
            : base(testMethod, @class, method, testCases, messageBus, aggregator, cancellationTokenSource)
        {
            this.serviceProvider = serviceProvider;
            this.diagnosticMessageSink = diagnosticMessageSink;
            this.constructorArguments = constructorArguments;
        }

        /// <inheritdoc />
        protected override async Task<RunSummary> RunTestCaseAsync(IXunitTestCase testCase)
        {
            if (testCase is ExecutionErrorTestCase)
            {
                return await testCase.RunAsync(
                        this.diagnosticMessageSink,
                        this.MessageBus,
                        this.constructorArguments,
                        new ExceptionAggregator(this.Aggregator),
                        this.CancellationTokenSource)
                    .ConfigureAwait(false);
            }

            XunitTestCaseRunner? runner;
            if (testCase is XunitTheoryTestCase)
            {
                runner = new XunitTheoryTestCaseRunner(
                    testCase,
                    testCase.DisplayName,
                    testCase.SkipReason,
                    this.constructorArguments,
                    this.diagnosticMessageSink,
                    this.MessageBus,
                    this.Aggregator,
                    this.CancellationTokenSource);
            }
            else
            {
                runner = new MettleTestCaseRunner(
                    this.serviceProvider,
                    testCase,
                    testCase.DisplayName,
                    testCase.SkipReason,
                    this.constructorArguments,
                    testCase.TestMethodArguments,
                    this.MessageBus,
                    this.Aggregator,
                    this.CancellationTokenSource);
            }

            var summary = await runner.RunAsync().ConfigureAwait(false);

            if (runner is IDisposable disposable)
                disposable.Dispose();

            return summary;
        }
    }
}