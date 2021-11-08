using System;
using Xunit.Sdk;

namespace Mettle
{
    /// <summary>
    /// Declares a method as an integration test.
    /// </summary>
    [AttributeUsage(
         AttributeTargets.Method,
         Inherited = false)]
    [XunitTestCaseDiscoverer(Util.TestCaseDiscoverer, Util.TestCaseDiscoverer)]
    public class IntegrationAttribute : TestAttribute
    {
        public IntegrationAttribute()
            : base("integration")
        {
        }
    }
}