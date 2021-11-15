using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NerdyMishka.Shell.Abstractions
{
    [AttributeUsage(
        AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class ShellArgumentTokenAttribute : Attribute
    {
        public int? Position { get; set; }

        public string? Name { get; set; }

        protected internal PropertyInfo? PropertyInfo { get; set; }
    }
}
