using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle.Sdk
{
    public class TestDiscoverer : FactDiscoverer
    {
        public TestDiscoverer(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink)
        {
        }

        public override IEnumerable<IXunitTestCase> Discover(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            ITestMethod testMethod,
            IAttributeInfo factAttribute)
        {
            if (factAttribute is ReflectionAttributeInfo reflect && reflect.Attribute is TestAttribute)
            {
                IXunitTestCase testCase;

                if (testMethod.Method.IsGenericMethodDefinition)
                    testCase = this.ErrorTestCase(discoveryOptions, testMethod, "[Fact] methods are not allowed to be generic.");
                else
                    testCase = this.CreateTestCase(discoveryOptions, testMethod, factAttribute);

                return new[] { testCase };
            }

            return base.Discover(discoveryOptions, testMethod, factAttribute);
        }

        protected override IXunitTestCase CreateTestCase(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            var category = factAttribute.GetNamedArgument<string?>("Category");
            var tags = factAttribute.GetNamedArgument<string[]?>("Tags");
            var ticketUri = factAttribute.GetNamedArgument<string?>("TicketUri");
            var ticketId = factAttribute.GetNamedArgument<string?>("TicketId");
            var ticketKind = factAttribute.GetNamedArgument<string?>("TicketKind");
            var documentUri = factAttribute.GetNamedArgument<string?>("DocumentUri");
            var traits = new Dictionary<string, List<string?>>();

            if (!string.IsNullOrWhiteSpace(category))
                traits.Add("Category", category);

            if (tags is { Length: > 0 })
            {
                foreach (var tag in tags)
                {
                    if (string.IsNullOrEmpty(tag))
                        continue;

                    traits.Add("tags", tag);
                }
            }

            if (!string.IsNullOrWhiteSpace(ticketUri))
                traits.Add("ticketUri", ticketUri);

            if (!string.IsNullOrWhiteSpace(ticketId))
                traits.Add("ticketId", ticketId);

            if (!string.IsNullOrWhiteSpace(ticketKind))
                traits.Add("ticketKind", ticketKind);

            if (!string.IsNullOrWhiteSpace(documentUri))
                traits.Add("documentUri", documentUri);

            var sb = new StringBuilder();
            foreach (var kvp in SkippableTraitAttribute.SkippableTraits)
            {
                var ai = testMethod.Method.GetCustomAttributes(kvp.Value);

                if (ai is not ReflectionAttributeInfo reflect)
                {
                    continue;
                }

                if (reflect.Attribute is not SkippableTraitAttribute attr)
                    continue;

                var nextReason = attr.GetSkipReason(this.DiagnosticMessageSink, testMethod, factAttribute);
                if (!string.IsNullOrWhiteSpace(nextReason))
                {
                    if (sb.Length > 0)
                        sb.Append(", ");
                    sb.Append(nextReason);
                }
            }

            var skipReason = sb.ToString();
            var test = new MettleTestCase(
                skipReason,
                traits,
                null,
                this.DiagnosticMessageSink,
                discoveryOptions.MethodDisplayOrDefault(),
                discoveryOptions.MethodDisplayOptionsOrDefault(),
                testMethod);

            return test;
        }

        private ExecutionErrorTestCase ErrorTestCase(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            ITestMethod testMethod,
            string message) =>
            new(
                this.DiagnosticMessageSink,
                discoveryOptions.MethodDisplayOrDefault(),
                discoveryOptions.MethodDisplayOptionsOrDefault(),
                testMethod,
                message);
    }
}