using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using NerdyMishka.Composition;

namespace NerdyMishka.CommandLine.Hosting
{
    public class SubCommandBuilder
    {
        private readonly CommandContainer container = new();

        private readonly ILogger? logger;

        public SubCommandBuilder(ILogger<SubCommandBuilder>? logger = null)
        {
            this.logger = logger;
        }

        public SubCommandBuilder Add(Assembly assembly)
        {
            if (!this.container.CommandAssemblies.Contains(assembly))
                this.container.CommandAssemblies.Add(assembly);

            return this;
        }

        public SubCommandBuilder Add(string directory, bool throwOnError = false)
        {
            if (Check.IsNullOrWhiteSpace(directory))
            {
                if (throwOnError)
                    throw new ArgumentNullException(nameof(directory));

                return this;
            }

            if (!Directory.Exists(directory))
            {
                if (throwOnError)
                    throw new DirectoryNotFoundException(directory);

                return this;
            }

            if (!this.container.ModuleDirectories.Contains(directory))
                this.container.ModuleDirectories.Add(directory);

            return this;
        }

        public SubCommandBuilder Add(Command command)
        {
            if (!this.container.CommandStore.Contains(command))
                this.container.CommandStore.Add(command);

            return this;
        }

        public IEnumerable<Command> Build()
        {
            this.container.Compose(this.logger);
            return this.container.SubCommands ?? Array.Empty<Command>();
        }
    }
}
