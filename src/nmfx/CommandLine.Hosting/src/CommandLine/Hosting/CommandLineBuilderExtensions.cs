using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace NerdyMishka.CommandLine.Hosting
{
    public static class CommandLineBuilderExtensions
    {
        private const string ConfigureBuilder = "ConfigureBuilder";

        public static Task<int> RunAsync(this CommandLineBuilder builder, IReadOnlyList<string> arguments,  IConsole? console = null)
        {
            var parser = builder.Build();
            return parser.Parse(arguments).InvokeAsync(console);
        }

        public static CommandLineBuilder UseStartup<T>(this CommandLineBuilder builder)
        {
            builder.UseStartup(typeof(T));
            return builder;
        }

        public static CommandLineBuilder UseStartup(
            this CommandLineBuilder builder,
            Type startupType)
        {
            var root = (RootCommand)builder.Command;
            var startup = Activator.CreateInstance(startupType);

            builder.UseHost(hb =>
            {
                hb.UseAppStartup(startup);
                hb.AddCommandHandlersFromAttribute(root);
            });

            var configureBuilderMethod = startupType.GetMethod(ConfigureBuilder);

            if (configureBuilderMethod != null)
            {
                var instance = configureBuilderMethod.IsStatic ? null : startup;
                var args = new object[] { builder };
                configureBuilderMethod.Invoke(instance, args);
            }

            return builder;
        }
    }
}
