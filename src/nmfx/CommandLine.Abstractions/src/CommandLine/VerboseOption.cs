using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace NerdyMishka.CommandLine
{
    public class VerboseOption : Option<bool>
    {
        public VerboseOption()
         : base(new string[] { "--verbose", "-v" }, "Prints out verbose messages")
        {
        }
    }
}
