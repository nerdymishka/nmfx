using System;
using Xunit;
using Xunit.Sdk;

namespace Mettle
{
    /// <summary>
    /// Abstract class for test case attributes that is derviced from the
    /// <see cref="FactAttribute"/>.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Method,
        Inherited = false)]
    [XunitTestCaseDiscoverer(Util.TestCaseDiscoverer, Util.AssemblyName)]
    public class TestAttribute : FactAttribute
    {
        public TestAttribute()
        {
            this.Category = "test";
        }

        /// <summary>
        /// Gets or sets a link to the ticket this test was created for.
        /// </summary>
        /// <value>
        /// Property <see cref="TicketUri" /> represents the uri of a ticket that the
        /// test is linked to.
        /// </value>
        public string? TicketUri { get; set; }

        /// <summary>
        /// Gets or sets the ticket id, which can be used as a filter.
        /// </summary>
        /// <value>
        /// Property <see cref="TicketId" /> represents the id of a ticket that the
        /// test is linked to which can be used to filter on when executing
        /// tests.
        /// </value>
        public string? TicketId { get; set; }

        /// <summary>
        /// Gets or sets the kind of the ticket such as bug, feature, etc.
        /// </summary>
        /// <value>
        /// Property <see cref="TicketKind" /> represents the kind of
        /// ticket the test is linked to.
        /// </value>
        public string? TicketKind { get; set; }

        /// <summary>Gets or sets the document uri.</summary>
        /// <value>
        /// Property  <see cref="DocumentUri" /> represents a link to document or page
        /// relevant to this test.
        /// </value>
        public string? DocumentUri { get; set; }

        /// <summary>
        /// Gets or sets the additional tags, which can
        /// be used as a filter.
        /// </summary>
        /// <value>
        /// Property <see cref="Tags" /> represents additional tags that stored as an
        /// xunit trait with the key 'tag'.  Traits can be used to filter
        /// which tests are executed at runtime.
        /// </value>
        public string[]? Tags { get; set; }

        protected internal string? Category { get; set; }
    }
}