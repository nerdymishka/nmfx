using System;
using System.Collections.Generic;
using System.CommandLine;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;

namespace NerdyMishka.Composition
{
    internal class CommandContainer
    {
        [ImportMany(typeof(Command))]
        public IEnumerable<Command>? SubCommands { get; set; }

        protected internal HashSet<string> ModuleDirectories { get; set; } = new HashSet<string>();

        protected internal HashSet<Assembly> CommandAssemblies { get; set; } = new HashSet<Assembly>();

        protected internal HashSet<Command> CommandStore { get; set; } = new HashSet<Command>();

        public void Compose(ILogger? logger)
        {
            var catalog = new AggregateCatalog();

            var i = 0;

            foreach (var moduleDirectory in this.ModuleDirectories)
            {
                if (Check.IsNullOrWhiteSpace(moduleDirectory))
                {
                    logger.LogWarning($"Module directory at index {i} is null or whitespace.");
                    i++;
                    continue;
                }

                if (!Directory.Exists(moduleDirectory))
                {
                    logger?.LogWarning($"Module directory does not exist: {moduleDirectory}");
                    i++;
                    continue;
                }

                catalog.Catalogs.Add(new DirectoryCatalog(moduleDirectory, "*.dll"));
            }

            foreach (var assembly in this.CommandAssemblies)
            {
                catalog.Catalogs.Add(new AssemblyCatalog(assembly));
            }

            using var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            var list = new List<Command>(this.SubCommands);
            list.AddRange(this.CommandStore);
            this.SubCommands = list;
        }
    }
}
