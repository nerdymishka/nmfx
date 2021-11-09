using System;
using System.Collections.Generic;
using System.Text;

namespace Mettle.Abstractions
{
    public interface IServiceProviderLifetimeFactory
    {
        IServiceProviderLifetime CreateLifetime();
    }
}
