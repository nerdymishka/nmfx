using System;
using System.Collections.Generic;
using System.Text;

namespace Mettle
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class InjectAttribute : Attribute
    {
        public InjectAttribute()
        {
        }
    }
}
