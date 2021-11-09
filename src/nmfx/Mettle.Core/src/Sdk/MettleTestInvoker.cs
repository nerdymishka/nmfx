using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Mettle.Abstractions;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle.Sdk
{
    /// <summary>
    /// The Mettle test invoker for xunit v2 tests. Invokes the test method. The last stop in the execution
    /// pipeline.
    /// </summary>
    public class MettleTestInvoker : XunitTestInvoker
    {
        private readonly IServiceProvider? serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MettleTestInvoker"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider for injecting dependencies.</param>
        /// <param name="test">The test that this invocation belongs to.</param>
        /// <param name="messageBus">The message bus to report run status to.</param>
        /// <param name="testClass">The test class that the test method belongs to.</param>
        /// <param name="constructorArguments">The arguments to be passed to the test class constructor.</param>
        /// <param name="testMethod">The test method that will be invoked.</param>
        /// <param name="testMethodArguments">The arguments to be passed to the test method.</param>
        /// <param name="beforeAfterAttributes">The list of <see cref="BeforeAfterTestAttribute"/>s for this test invocation.</param>
        /// <param name="aggregator">The exception aggregator used to run code and collect exceptions.</param>
        /// <param name="cancellationTokenSource">The task cancellation token source, used to cancel the test run.</param>
        public MettleTestInvoker(
            IServiceProvider? serviceProvider,
            ITest test,
            IMessageBus messageBus,
            Type testClass,
            object?[]? constructorArguments,
            MethodInfo testMethod,
            object[] testMethodArguments,
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
                beforeAfterAttributes,
                aggregator,
                cancellationTokenSource)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        protected override object CallTestMethod(object testClassInstance)
        {
            var result = base.CallTestMethod(testClassInstance);

            return result is Task task ? AsyncStack(task) : result;

            async Task AsyncStack(Task t)
            {
                try
                {
                    await t.ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    while (ex is TargetInvocationException { InnerException: { } } targetInvocationException)
                    {
                        ex = targetInvocationException.InnerException;
                    }

                    if (this.serviceProvider?.GetService(typeof(IExceptionFilter)) is IExceptionFilter exceptionFilter)
                    {
                        ex = exceptionFilter.Filter(ex);
                    }

                    this.Aggregator.Add(ex);
                }
            }
        }
    }
}
