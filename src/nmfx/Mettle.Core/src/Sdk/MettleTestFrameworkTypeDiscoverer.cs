using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle.Sdk
{
    public class MettleTestFrameworkTypeDiscoverer : ITestFrameworkTypeDiscoverer
    {
        public Type GetTestFrameworkType(IAttributeInfo attribute)
        {
            var frameworkType = attribute.GetNamedArgument<Type?>("CustomFrameworkType");
            return frameworkType ?? typeof(MettleTestFramework);
        }
    }
}
