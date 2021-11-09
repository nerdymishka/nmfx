using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mettle.Abstractions;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle.Sdk
{
    /// <summary>
    /// The Mettle test runner for Xunit v2 tests. The 2nd to last point in the execution pipeline.
    /// This class calls <see cref="MettleTestInvoker"/>.
    /// </summary>
    public class MettleTestRunner : XunitTestRunner
    {
        private readonly IServiceProvider? serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MettleTestRunner"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider for injecting dependencies.</param>
        /// <param name="test">The test that this invocation belongs to.</param>
        /// <param name="messageBus">The message bus to report run status to.</param>
        /// <param name="testClass">The test class that the test method belongs to.</param>
        /// <param name="constructorArguments">The arguments to be passed to the test class constructor.</param>
        /// <param name="testMethod">The test method that will be invoked.</param>
        /// <param name="testMethodArguments">The arguments to be passed to the test method.</param>
        /// <param name="skipReason">The skip reason, if the test is to be skipped.</param>
        /// <param name="beforeAfterAttributes">The list of <see cref="BeforeAfterTestAttribute"/>s for this test.</param>
        /// <param name="aggregator">The exception aggregator used to run code and collect exceptions.</param>
        /// <param name="cancellationTokenSource">The task cancellation token source, used to cancel the test run.</param>
        public MettleTestRunner(
            IServiceProvider? serviceProvider,
            ITest test,
            IMessageBus messageBus,
            Type testClass,
            object?[] constructorArguments,
            MethodInfo testMethod,
            object[] testMethodArguments,
            string skipReason,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(
                test,
                messageBus,
                testClass,
                constructorArguments,
                testMethod,
                testMethodArguments,
                skipReason,
                beforeAfterAttributes,
                aggregator,
                cancellationTokenSource)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        protected override async Task<Tuple<decimal, string>> InvokeTestAsync(ExceptionAggregator aggregator)
        {
            var output = string.Empty;

            ITestOutputHelper? testOutputHelper = null;
            foreach (var obj in this.ConstructorArguments)
            {
                if (obj is ITestOutputHelper outputHelper)
                {
                    testOutputHelper = outputHelper;
                    break;
                }
            }

            foreach (var obj in this.TestMethodArguments)
            {
                if (obj is ITestOutputHelper outputHelper)
                {
                    testOutputHelper = outputHelper;
                    break;
                }
            }

            if (testOutputHelper is null &&
                this.serviceProvider?.GetService(typeof(ITestOutputHelperContainer))
                is ITestOutputHelperContainer container)
            {
                testOutputHelper = container.TestOutputHelper;
            }

            var xunitOutputHelper = testOutputHelper as TestOutputHelper;
            xunitOutputHelper?.Initialize(this.MessageBus, this.Test);

            var executionTime = await this.InvokeTestMethodAsync(aggregator)
                .ConfigureAwait(false);

            if (xunitOutputHelper != null)
            {
                output = xunitOutputHelper.Output;
                xunitOutputHelper.Uninitialize();
            }

            return Tuple.Create(executionTime, output);
        }
    }
}
