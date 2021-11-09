using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Mettle.Abstractions;
using Xunit.Abstractions;

namespace Mettle.Sdk
{
    public class TestOutputHelperContainer : ITestOutputHelperContainer
    {
        private readonly AsyncLocal<ITestOutputHelper?> outputHelper = new();

        public ITestOutputHelper? TestOutputHelper
        {
            get => this.outputHelper.Value;
            set => this.outputHelper.Value = value;
        }
    }
}
