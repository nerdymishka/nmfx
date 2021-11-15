using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NerdyMishka.Util;

namespace NerdyMishka.CommandLine.Hosting
{
    public static class HostBuilderExtensions
    {
        private const string ConfigureHostConfiguration = "ConfigureHostConfiguration";
        private const string ConfigureAppConfiguration = "ConfigureAppConfiguration";
        private const string ConfigureServices = "ConfigureServices";
        private const string ConfigureContainer = "ConfigureContainer";
        private const string ConfigureLogging = "ConfigureLogging";

        public static IHostBuilder AddCommandHandlersFromAttribute(this IHostBuilder hostBuilder, RootCommand rootCommand)
        {
            var commands = FlattenAllCommands(rootCommand);

            if (hostBuilder.Properties[typeof(InvocationContext)] is InvocationContext invocationContext
                && invocationContext.ParseResult.CommandResult.Command is Command command)
            {
                foreach (var cmd in commands)
                {
                    if (cmd == command)
                    {
                        RegisterCommandHandlerFromAttribute(hostBuilder, cmd);
                        return hostBuilder;
                    }
                }
            }
            else
            {
                foreach (var cmd in commands)
                {
                    RegisterCommandHandlerFromAttribute(hostBuilder, cmd);
                }
            }

            return hostBuilder;
        }

        public static IHostBuilder UseAppStartup(this IHostBuilder hostBuilder, Type startupType)
        {
            var services = hostBuilder.GetInvocationContext()?.GetHost().Services;
            object startup;
            if (services != null)
            {
                startup = ActivatorUtilities.CreateInstance(services, startupType);
            }
            else
            {
                startup = Activator.CreateInstance(startupType);
            }

            UseAppStartup(hostBuilder, startup);

            return hostBuilder;
        }

        public static IHostBuilder UseAppStartup(this IHostBuilder hostBuilder, object startup)
        {
            hostBuilder.Properties["Startup"] = startup;
            var methods = startup.GetType().GetMethods();
            var configureHostConfigurationMethod = methods.SingleOrDefault(o => o.Name.EqualsCaseInsensitive(ConfigureHostConfiguration));
            var configureAppConfigurationMethod = methods.SingleOrDefault(o => o.Name.EqualsCaseInsensitive(ConfigureAppConfiguration));
            var configureServicesMethod = methods.SingleOrDefault(o => o.Name.EqualsCaseInsensitive(ConfigureServices));
            var configureContainerMethod = methods.SingleOrDefault(o => o.Name.EqualsCaseInsensitive(ConfigureContainer));
            var configureLoggingMethod = methods.SingleOrDefault(o => o.Name.EqualsCaseInsensitive(ConfigureLogging));
            if (configureHostConfigurationMethod != null)
            {
                hostBuilder.ConfigureHostConfiguration(cb =>
                {
                    if (configureHostConfigurationMethod.IsStatic)
                    {
                        configureHostConfigurationMethod.Invoke(null, new object[] { cb });
                    }

                    configureHostConfigurationMethod.Invoke(startup, new object[] { cb });
                });
            }

            if (configureAppConfigurationMethod != null)
            {
                var parameters = configureAppConfigurationMethod.GetParameters();
                var instance = configureAppConfigurationMethod.IsStatic ? null : startup;
                if (parameters.Length > 0 && parameters.Length < 3)
                {
                    hostBuilder.ConfigureAppConfiguration((ctx, cb) =>
                    {
                        var arguments = parameters.Length == 2 ? new object[] { ctx, cb } : new object[] { cb };
                        configureAppConfigurationMethod.Invoke(instance, arguments);
                    });
                }
            }

            if (configureContainerMethod != null)
            {
                var parameters = configureContainerMethod.GetParameters();

                if (parameters.Length > 0 && parameters.Length < 3)
                {
                    var containerBuilderType = parameters.Select(o => o.ParameterType).LastOrDefault();
                    if (containerBuilderType != null)
                    {
                        var hostBuliderMethod = hostBuilder.GetType().GetMethod(nameof(HostBuilder.ConfigureContainer), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                            .MakeGenericMethod(containerBuilderType);

                        Type delegateType;

                        if (parameters.Length == 2)
                            delegateType = typeof(Action<,>).MakeGenericType(typeof(HostBuilderContext), containerBuilderType);
                        else
                            delegateType = typeof(Action<>).MakeGenericType(containerBuilderType);

                        var action = Delegate.CreateDelegate(delegateType, startup, hostBuliderMethod);

                        hostBuliderMethod.Invoke(hostBuilder, new object[] { action });
                    }
                }
            }

            if (configureServicesMethod != null)
            {
                var parameters = configureServicesMethod.GetParameters();
                var instance = configureServicesMethod.IsStatic ? null : startup;
                if (parameters.Length > 0 && parameters.Length < 3)
                {
                    hostBuilder.ConfigureServices((ctx, services) =>
                    {
                        var arguments = parameters.Length == 2 ? new object[] { ctx, services } : new object[] { services };
                        configureServicesMethod.Invoke(instance, arguments);
                    });
                }
            }

            if (configureLoggingMethod != null)
            {
                var parameters = configureLoggingMethod.GetParameters();
                var instance = configureLoggingMethod.IsStatic ? null : startup;
                if (parameters.Length > 0 && parameters.Length < 3)
                {
                    hostBuilder.ConfigureLogging((ctx, lb) =>
                    {
                        var arguments = parameters.Length == 2 ? new object[] { ctx, lb } : new object[] { lb };
                        configureLoggingMethod.Invoke(instance, arguments);
                    });
                }
            }

            return hostBuilder;
        }

        private static void RegisterCommandHandlerFromAttribute(IHostBuilder hostBuilder, Command cmd)
        {
            if (hostBuilder is null)
                throw new ArgumentNullException(nameof(hostBuilder));

            if (cmd is null)
                throw new ArgumentNullException(nameof(cmd));

            var commandType = cmd.GetType();
            var attr = commandType.GetCustomAttribute<CommandHandlerAttribute>();

            if (attr is null || attr.CommandHandlerType is null)
                return;

            var objType = typeof(object);
            var invocationContext = hostBuilder.GetInvocationContext();
            var bt = attr.CommandHandlerType.BaseType;
            invocationContext
                .BindingContext
                    .AddService(
                        attr.CommandHandlerType,
                        (s) => s.GetService<IHost>().Services.GetService(attr.CommandHandlerType));

            while (bt != null && bt != objType)
            {
                invocationContext
               .BindingContext
                   .AddService(
                       attr.CommandHandlerType,
                       (s) => s.GetService<IHost>().Services.GetService(attr.CommandHandlerType));

                bt = bt.BaseType;
            }

            hostBuilder.ConfigureServices(s => s.AddTransient(attr.CommandHandlerType));
            cmd.Handler = CommandHandler.Create(attr.CommandHandlerType.GetMethod(nameof(ICommandHandler.InvokeAsync)));
        }

        private static IEnumerable<Command> FlattenAllCommands(Command command)
        {
            var result = new List<Command>();
            foreach (var child in command.Children)
            {
                if (child is Command subCommand)
                {
                    result.Add(subCommand);
                    var grandChildren = FlattenAllCommands(subCommand);
                    result.AddRange(grandChildren);
                }
            }

            return result;
        }
    }
}
