using System;
using System.Collections.Generic;
using System.Text;

namespace Mettle.Abstractions
{
    public interface IExceptionFilter
    {
        Exception Filter(Exception ex);
    }
}
