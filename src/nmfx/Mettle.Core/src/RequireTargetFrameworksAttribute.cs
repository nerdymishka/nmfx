using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Mettle.Sdk;
using NuGet.Frameworks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle
{
    [System.AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Method,
        Inherited = false,
        AllowMultiple = true)]
    public sealed class RequireTargetFrameworksAttribute : SkippableTraitAttribute
    {
        private static readonly ConcurrentDictionary<string, NuGetFramework> Assemblies = new();

        static RequireTargetFrameworksAttribute()
        {
            AddTraitAttribute(typeof(RequireTargetFrameworksAttribute));
        }

        public RequireTargetFrameworksAttribute(params string[] targetFrameworks)
        {
            if (targetFrameworks == null || targetFrameworks.Length == 0)
            {
                this.TargetFrameworks = Array.Empty<string>();
                return;
            }

            this.TargetFrameworks = targetFrameworks
                .Where(o => !string.IsNullOrWhiteSpace(o))
                .ToArray();
        }

        public string[] TargetFrameworks { get; set; }

        public override string? GetSkipReason(IMessageSink sink, ITestMethod testMethod, IAttributeInfo attributeInfo)
        {
            var name = testMethod.TestClass.Class.Assembly.Name;
            if (!Assemblies.TryGetValue(name, out var targetFramework))
            {
                try
                {
                    var assemblyName = new AssemblyName(testMethod.TestClass.Class.Assembly.Name);
                    var assembly = Assembly.Load(assemblyName);
                    targetFramework = DiscovererHelpers.GetNuGetFramework(assembly);
                    Assemblies.TryAdd(name, targetFramework);
                }
                catch (Exception ex)
                {
                    sink.OnMessage(new DiagnosticMessage(ex.Message + Environment.NewLine + ex.StackTrace));
                    return $"Require target frameworks error: {ex.Message}";
                }
            }

            if (this.TargetFrameworks == null || this.TargetFrameworks.Length == 0)
                return null;

            foreach (var target in this.TargetFrameworks)
            {
                if (string.IsNullOrWhiteSpace(target))
                    continue;

                if (targetFramework.IsAny)
                    return null;

                var parts = target.Split(' ');
                var match = "==";
                var nextTarget = target;

                try
                {
                    if (parts.Length == 2)
                    {
                        match = parts[0];
                        nextTarget = parts[1];
                    }
                    else
                    {
                        var framework1 = NuGet.Frameworks.NuGetFramework.Parse(nextTarget);
                        var compat = DefaultCompatibilityProvider.Instance;
                        if (compat.IsCompatible(framework1, targetFramework))
                            return null;
                    }

                    var framework = NuGetFramework.Parse(nextTarget);

                    switch (match)
                    {
                        case "==":
                            if (framework == targetFramework)
                                return null;

                            break;
                        case ">=":
                            if (framework.Framework == targetFramework.Framework
                                && framework.Version >= targetFramework.Version)
                            {
                                return null;
                            }

                            break;
                        case ">":
                            if (framework.Framework == targetFramework.Framework
                                && framework.Version > targetFramework.Version)
                            {
                                return null;
                            }

                            break;
                        case "!=":
                            if (framework.Framework != targetFramework.Framework
                                || framework.Version != targetFramework.Version)
                            {
                                return null;
                            }

                            break;
                        case "<=":
                            if (framework.Framework == targetFramework.Framework
                                && framework.Version <= targetFramework.Version)
                            {
                                return null;
                            }

                            break;
                        case "<":
                            if (framework.Framework == targetFramework.Framework
                                && framework.Version >= targetFramework.Version)
                            {
                                return null;
                            }

                            break;
                        default:
                            if (framework == targetFramework)
                                return null;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    sink.OnMessage(new DiagnosticMessage(ex.Message + Environment.NewLine + ex.StackTrace));
                    return $"Require target frameworks error: {ex.Message}";
                }
            }

            return $"Requires target frameworks: {string.Join(",", this.TargetFrameworks)}";
        }
    }
}