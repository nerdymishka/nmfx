using System.Collections.Generic;
using System.Text;
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

            if (tags != null && tags.Length > 0)
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
                var attr = testMethod
                    .Method
                    .GetCustomAttributes(typeof(RequireOsPlatformsAttribute))
                    as SkippableTraitAttribute;

                if (attr is null)
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
    }
}