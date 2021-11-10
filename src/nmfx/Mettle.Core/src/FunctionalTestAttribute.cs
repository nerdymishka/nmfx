using System;
using Xunit.Sdk;

namespace Mettle
{
    /// <summary>
    /// Declares a method as a functional test.
    /// </summary>
    [AttributeUsage(
         AttributeTargets.Method,
         Inherited = false)]
    [XunitTestCaseDiscoverer(Util.TestCaseDiscoverer, Util.AssemblyName)]
    public class FunctionalTestAttribute : TestAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalTestAttribute"/> class.
        /// </summary>
        public FunctionalTestAttribute()
            : base("functional")
        {
        }
    }
}