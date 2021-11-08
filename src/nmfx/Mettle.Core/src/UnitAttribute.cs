using System;
using System.Linq;
using Xunit.Sdk;

namespace Mettle
{
    /// <summary>
    /// Declares a method as a unit test.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Method,
        Inherited = false)]
    [XunitTestCaseDiscoverer(Util.TestCaseDiscoverer, Util.AssemblyName)]
    public class UnitAttribute : TestAttribute
    {
        public UnitAttribute()
            : base("unit")
        {
        }
    }
}