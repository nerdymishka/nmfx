using System;
using System.Reflection;
using Xunit.Abstractions;

namespace Mettle
{
#pragma warning disable REFL016

    [System.AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Method,
        Inherited = false,
        AllowMultiple = true)]
    public sealed class RequireOsArchitecturesAttribute : SkippableTraitAttribute
    {
        private static readonly OsArchitectures Arch = SetOsArch();

        public RequireOsArchitecturesAttribute(params string[] osArchitectures)
        {
            OsArchitectures arches = OsArchitectures.None;
            foreach (var arch in osArchitectures)
            {
                switch (arch)
                {
                    case "Any":
                        this.OsArchitectures = OsArchitectures.None;
                        return;

                    default:
                        if (Enum.TryParse<OsArchitectures>(arch, true, out var next))
                        {
                            if (arches == OsArchitectures.None)
                                arches = next;
                            else
                                arches |= next;
                        }

                        break;
                }
            }

            this.OsArchitectures = arches;
        }

        public RequireOsArchitecturesAttribute(OsArchitectures osArchitectures)
        {
            if (osArchitectures.HasFlag(OsArchitectures.None))
            {
                this.OsArchitectures = OsArchitectures.None;
                return;
            }

            this.OsArchitectures = osArchitectures;
        }

        public OsArchitectures OsArchitectures { get; set; }

        public override string? GetSkipReason(IMessageSink sink, ITestMethod testMethod, IAttributeInfo attributeInfo)
        {
            if (this.OsArchitectures.HasFlag(OsArchitectures.None))
                return null;

            if (this.OsArchitectures.HasFlag(Arch))
                return null;

            return $"Requires OS Architectures: {this.OsArchitectures}";
        }

        private static OsArchitectures SetOsArch()
        {
            AddTraitAttribute(typeof(RequireOsArchitecturesAttribute));
            string? arch = Environment.Is64BitOperatingSystem ? "X64" : "X86";
            try
            {
                var t = typeof(System.Runtime.InteropServices.RuntimeInformation);
                var osArchProp = t.GetProperty("OSArchitecture", BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
                if (osArchProp != null)
                {
                    var runtimeArch = osArchProp.GetValue(null);
                    if (runtimeArch != null)
                    {
                        arch = runtimeArch.ToString();
                    }
                }
            }
            catch
            {
                arch = Environment.Is64BitOperatingSystem ? "X64" : "X86";
            }

            if (Enum.TryParse<OsArchitectures>(arch, true, out var next))
            {
                return next;
            }
            else
            {
                return Environment.Is64BitOperatingSystem ? OsArchitectures.X64 : OsArchitectures.X86;
            }
        }
    }
}