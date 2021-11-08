using System;
using System.Collections.Generic;
using System.Text;

namespace Mettle.Sdk
{
    public interface IExceptionFilter
    {
        Exception Filter(Exception ex);
    }
}
