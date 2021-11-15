using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka
{
    [System.AttributeUsage(AttributeTargets.Class)]
    public class CommandHandlerAttribute : Attribute
    {
        public CommandHandlerAttribute(Type commandHandlerType)
        {
            this.CommandHandlerType = commandHandlerType;
        }

        public Type CommandHandlerType { get; set; }
    }
}
