using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace NerdyMishka.CommandLine
{
    public class DebugOption : Option<bool>
    {
        public DebugOption()
           : base(new string[] { "--debug", "-d" }, "Prints out debug messages")
        {
        }
    }
}
