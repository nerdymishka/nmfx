using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace NerdyMishka.CommandLine.Hosting
{
    public static class RootCommandExtensions
    {
        public static RootCommand WithCommandBuilder(this RootCommand rootCommand, Action<SubCommandBuilder> modify)
        {
            var builder = new SubCommandBuilder();
            modify?.Invoke(builder);

            var commands = builder.Build();
            foreach (var command in commands)
                rootCommand.Add(command);

            return rootCommand;
        }
    }
}
