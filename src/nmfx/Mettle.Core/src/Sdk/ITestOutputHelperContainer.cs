using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;

namespace Mettle.Sdk
{
    public interface ITestOutputHelperContainer
    {
        public ITestOutputHelper? TestOutputHelper { get; set; }
    }
}
