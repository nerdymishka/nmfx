using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle.Sdk
{
    public static class MettleServiceProviderLocator
    {
        private static readonly ConcurrentDictionary<Type?, IServiceProvider?> Cache = new();

        private static readonly IServiceProvider DefaultServiceProvider = new MettleServiceProvider();

        public static IServiceProvider? GetServiceProvider(IAssemblyInfo assemblyInfo)
        {
            var attributes = assemblyInfo.GetCustomAttributes(typeof(UseServiceProviderFactoryAttribute));
            var attribute = attributes.FirstOrDefault();

            if (attribute == null)
                return null;

            return GetServiceProvider(attribute);
        }

        public static IServiceProvider? GetServiceProvider(ITestClass @class)
        {
            var attributes = @class.Class.GetCustomAttributes(typeof(UseServiceProviderFactoryAttribute));
            var attribute = attributes.FirstOrDefault();

            if (attribute == null)
                return null;

            return GetServiceProvider(attribute);
        }

        public static IServiceProvider? GetServiceProvider(ITestMethod method)
        {
            var attributes = method.Method.GetCustomAttributes(typeof(UseServiceProviderFactoryAttribute));
            var attribute = attributes.FirstOrDefault();
            if (attribute == null)
            {
                attributes = method.TestClass.Class.GetCustomAttributes(typeof(UseServiceProviderFactoryAttribute));
                attribute = attributes.FirstOrDefault();

                if (attribute == null)
                    return null;
            }

            return GetServiceProvider(attribute);
        }

        public static IServiceProvider? GetServiceProvider(IAttributeInfo attributeInfo)
        {
            var serviceProviderFactoryType = attributeInfo.GetNamedArgument<Type>(nameof(UseServiceProviderFactoryAttribute.FactoryType));
            return GetServiceProvider(serviceProviderFactoryType);
        }

        public static IServiceProvider? GetServiceProvider(Type? serviceProviderFactoryType, IMessageSink? sink = null)
        {
            if (serviceProviderFactoryType is null)
                return DefaultServiceProvider;

            if (Cache.TryGetValue(serviceProviderFactoryType, out var serviceProvider) && serviceProvider != null)
                return serviceProvider;

            try
            {
                if (Activator.CreateInstance(serviceProviderFactoryType) is IMettleServiceProviderFactory factory)
                {
                    serviceProvider = factory.CreateServiceProvider();
                    Cache.TryAdd(serviceProviderFactoryType, serviceProvider);
                    return serviceProvider;
                }
            }
            catch (Exception ex)
            {
                sink?.OnMessage(new DiagnosticMessage(
                    $"exception thrown when creating serviceProvider from {serviceProviderFactoryType.FullName} {ex.Message} {ex.StackTrace}"));
            }

            Cache.TryAdd(serviceProviderFactoryType, DefaultServiceProvider);
            return DefaultServiceProvider;
        }
    }
}
