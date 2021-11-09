using System;
using Xunit.Sdk;

namespace Mettle
{
    [TestFrameworkDiscoverer(
        Util.TestFrameworkTypeDiscoverer,
        Util.AssemblyName)]
    [AttributeUsage(
        System.AttributeTargets.Assembly,
        Inherited = false,
        AllowMultiple = false)]
    public sealed class MettleTestFrameworkAttribute : System.Attribute,
        ITestFrameworkAttribute
    {
        public MettleTestFrameworkAttribute()
        {
        }

        public Type? CustomFrameworkType { get; set; }
    }
}
