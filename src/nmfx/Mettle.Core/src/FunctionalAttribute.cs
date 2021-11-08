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
    public class FunctionalAttribute : TestAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalAttribute"/> class.
        /// </summary>
        public FunctionalAttribute()
            : base("functional")
        {
        }
    }
}