using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace NerdyMishka.CommandLine
{
    public class WhatIfOption : Option<bool>
    {
        public WhatIfOption()
            : base(
                new string[] { "--what-if", "--whatif", "-wi" },
                "Performs no operations and writes out what would happen if the command was executed.")
        {
        }
    }
}
