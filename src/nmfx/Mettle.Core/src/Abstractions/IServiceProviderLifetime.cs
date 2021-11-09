using System;
using System.Collections.Generic;
using System.Text;

namespace Mettle.Abstractions
{
    public interface IServiceProviderLifetime : IDisposable
    {
        IServiceProvider ServiceProvider { get; }
    }
}
