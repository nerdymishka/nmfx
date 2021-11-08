using System;
using Xunit;
using Xunit.Sdk;

namespace Mettle
{
    /// <summary>
    /// Core test attribute that is derived from the
    /// <see cref="FactAttribute"/> and is the base class for mettle
    /// test case attributes.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Method,
        Inherited = false)]
    [XunitTestCaseDiscoverer(Util.TestCaseDiscoverer, Util.AssemblyName)]
    public class TestAttribute : FactAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestAttribute"/> class.
        /// </summary>
        public TestAttribute()
            : this("test")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestAttribute"/> class.
        /// </summary>
        /// <param name="categoryName">The category of the test.</param>
        protected TestAttribute(string categoryName)
        {
            this.Category = categoryName;
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
        /// Xunit trait with the key 'tag'.  Traits can be used to filter
        /// which tests are executed at runtime.
        /// </value>
        public string[]? Tags { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        protected internal string? Category { get; set; }
    }
}