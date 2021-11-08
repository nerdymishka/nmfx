using System;
using System.Collections.Generic;
using System.Text;

namespace Mettle
{
    public interface IMettleServiceProviderFactory
    {
        IServiceProvider CreateServiceProvider();
    }
}
