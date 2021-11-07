using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using NuGet.Frameworks;

namespace Mettle.Sdk
{
    internal static class DiscovererHelpers
    {
        public static bool TestPlatformApplies(OsPlatforms platforms) =>
                platforms.HasFlag(OsPlatforms.None) ||
                (platforms.HasFlag(OsPlatforms.Windows) && RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) ||
                (platforms.HasFlag(OsPlatforms.Linux) && RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) ||
                (platforms.HasFlag(OsPlatforms.OSX) && RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) ||
                (platforms.HasFlag(OsPlatforms.IOS) && RuntimeInformation.IsOSPlatform(OSPlatform.Create("IOS")) && !RuntimeInformation.IsOSPlatform(OSPlatform.Create("MACCATALYST"))) ||
                (platforms.HasFlag(OsPlatforms.FreeBSD) && RuntimeInformation.IsOSPlatform(OSPlatform.Create("FREEBSD"))) ||
                (platforms.HasFlag(OsPlatforms.Android) && RuntimeInformation.IsOSPlatform(OSPlatform.Create("ANDROID"))) ||
                (platforms.HasFlag(OsPlatforms.NetBSD) && RuntimeInformation.IsOSPlatform(OSPlatform.Create("NETBSD"))) ||
                (platforms.HasFlag(OsPlatforms.Illumos) && RuntimeInformation.IsOSPlatform(OSPlatform.Create("ILLUMOS"))) ||
                (platforms.HasFlag(OsPlatforms.Solaris) && RuntimeInformation.IsOSPlatform(OSPlatform.Create("SOLARIS"))) ||
                (platforms.HasFlag(OsPlatforms.TVOS) && RuntimeInformation.IsOSPlatform(OSPlatform.Create("TVOS"))) ||
                (platforms.HasFlag(OsPlatforms.MacCatalyst) && RuntimeInformation.IsOSPlatform(OSPlatform.Create("MACCATALYST"))) ||
                (platforms.HasFlag(OsPlatforms.Browser) && RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER")));

        public static NuGetFramework GetNuGetFramework(Assembly? assembly = null)
        {
            assembly ??= System.Reflection.Assembly.GetEntryAssembly();
            var frameworkDescription = assembly?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;

            if (frameworkDescription != null)
            {
                return new NuGetFramework(frameworkDescription);
            }

            // disable reflection warning as this will get invoked on the full framework when TargetFrameworkAttribute does not exist.
#pragma warning disable REFL003, REFL016
            var setupProp = typeof(AppDomain).GetProperty("SetupInformation", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            if (setupProp != null)
            {
                var setup = setupProp.GetValue(AppDomain.CurrentDomain);
                var targetFrameworkProp = setup?.GetType().GetProperty("TargetFrameworkName");
                if (targetFrameworkProp != null)
                {
                    frameworkDescription = targetFrameworkProp.GetValue(setup) as string;
                    if (!string.IsNullOrWhiteSpace(frameworkDescription))
                    {
                        return new NuGetFramework(frameworkDescription);
                    }
                }
            }

            return NuGetFramework.UnsupportedFramework;
        }

        public static bool RuntimeConfigurationApplies(RuntimeConfigurations configurations) =>
            configurations.HasFlag(RuntimeConfigurations.None) ||
            (configurations.HasFlag(RuntimeConfigurations.Release) && IsReleaseRuntime()) ||
            (configurations.HasFlag(RuntimeConfigurations.Debug) && IsDebugRuntime()) ||
            (configurations.HasFlag(RuntimeConfigurations.Checked) && IsCheckedRuntime());

        private static bool IsCheckedRuntime() => AssemblyConfigurationEquals("Checked");

        private static bool IsReleaseRuntime() => AssemblyConfigurationEquals("Release");

        private static bool IsDebugRuntime() => AssemblyConfigurationEquals("Debug");

        private static bool AssemblyConfigurationEquals(string configuration)
        {
            var assemblyConfigurationAttribute = typeof(string).Assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();

            return assemblyConfigurationAttribute != null &&
                string.Equals(assemblyConfigurationAttribute.Configuration, configuration, StringComparison.InvariantCulture);
        }
    }
}