using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mettle.Abstractions;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle.Sdk
{
    public class MettleTheoryTestCaseRunner : XunitTheoryTestCaseRunner, IDisposable
    {
        private readonly IServiceProvider? serviceProvider;
        private readonly IDisposable? scope;

        public MettleTheoryTestCaseRunner(
            IServiceProvider? serviceProvider,
            IXunitTestCase testCase,
            string displayName,
            string skipReason,
            object[] constructorArguments,
            IMessageSink diagnosticMessageSink,
            IMessageBus messageBus,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(
                testCase,
                displayName,
                skipReason,
                constructorArguments,
                diagnosticMessageSink,
                messageBus,
                aggregator,
                cancellationTokenSource)
        {
            this.DisplayName = displayName;
            this.SkipReason = skipReason;
            this.TestClass = this.TestCase.TestMethod.TestClass.Class.ToRuntimeType();
            this.TestMethod = this.TestCase.Method.ToRuntimeMethod();
            this.serviceProvider = serviceProvider;

            if (this.serviceProvider?
                    .GetService(typeof(IServiceProviderLifetimeFactory))
                    is IServiceProviderLifetimeFactory factory)
            {
                var lifetime = factory.CreateLifetime();
                this.serviceProvider = lifetime.ServiceProvider;
                this.scope = lifetime;
            }

            var ctor = this.TestClass.GetConstructors().FirstOrDefault();

            if (constructorArguments.Length > 0)
            {
                var parameters = ctor?.GetParameters() ?? Array.Empty<ParameterInfo>();
                var parameterTypes = new Type[parameters.Length];
                for (var i = 0; i < parameters.Length; i++)
                    parameterTypes[i] = parameters[i].ParameterType;

                var ctorArgs = new object?[parameters.Length];
                Array.Copy(constructorArguments, ctorArgs, constructorArguments.Length);

                for (var i = 0; i < parameters.Length; i++)
                {
                    var obj = ctorArgs[i] ?? this.serviceProvider?.GetService(parameters[i].ParameterType);

                    ctorArgs[i] = obj;
                }

                this.ConstructorArguments = Reflector.ConvertArguments(ctorArgs, parameterTypes);
            }
            else
            {
                this.ConstructorArguments = constructorArguments;
            }

            this.MessageBus = new SkippedTestMessageBus(this.MessageBus);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected override XunitTestRunner CreateTestRunner(
            ITest test,
            IMessageBus messageBus,
            Type testClass,
            object[] constructorArguments,
            MethodInfo testMethod,
            object?[] testMethodArguments,
            string skipReason,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            var parameters = testMethod.GetParameters();
            var parameterTypes = new Type[parameters?.Length ?? 0];
            var args = testMethodArguments ?? Array.Empty<object>();
            if (parameters != null && parameters.Length != args.Length)
            {
                for (var i = 0; i < parameters.Length; i++)
                    parameterTypes[i] = parameters[i].ParameterType;

                var methodArgs = new object?[parameters.Length];
                Array.Copy(args, methodArgs, args.Length);
                for (var i = 0; i < parameters.Length; i++)
                {
                    var obj = methodArgs[i];
                    if (obj == null)
                    {
                        var p = parameters[i];
                        if (p.GetCustomAttributes(typeof(InjectAttribute), true).Length > 0)
                        {
                            obj = p.ParameterType.FullName switch
                            {
                                "System.IServiceProvider" => this.serviceProvider,
                                "Mettle.Sdk.MettleTestCase" => (MettleTestCase)this.TestCase,
                                "Xunit.Sdk.IXunitTestCase" => this.TestCase,
                                _ => this.serviceProvider?.GetService(p.ParameterType),
                            };
                        }
                    }

                    methodArgs[i] = obj;
                }

                testMethodArguments = Reflector.ConvertArguments(args, parameterTypes);
            }

            return base.CreateTestRunner(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, skipReason, beforeAfterAttributes, aggregator, cancellationTokenSource);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            this.scope?.Dispose();
        }
    }
}
