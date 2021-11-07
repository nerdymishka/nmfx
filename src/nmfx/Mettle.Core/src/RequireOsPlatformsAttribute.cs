using System;
using Mettle.Sdk;
using Xunit.Abstractions;

namespace Mettle
{
    [System.AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Method,
        Inherited = false,
        AllowMultiple = true)]
    public sealed class RequireOsPlatformsAttribute : SkippableTraitAttribute
    {
        static RequireOsPlatformsAttribute()
        {
            AddTraitAttribute(typeof(RequireOsPlatformsAttribute));
        }

        public RequireOsPlatformsAttribute(OsPlatforms osPlatforms)
        {
            if (osPlatforms.HasFlag(OsPlatforms.None))
            {
                this.OsPlatforms = OsPlatforms.None;
                return;
            }

            this.OsPlatforms = osPlatforms;
        }

        public RequireOsPlatformsAttribute(params string[] osPlatforms)
        {
            OsPlatforms platforms = OsPlatforms.None;
            foreach (var platform in osPlatforms)
            {
                switch (platform)
                {
                    case "Any":
                        this.OsPlatforms = OsPlatforms.None;
                        return;

                    default:
                        if (Enum.TryParse<OsPlatforms>(platform, out var next))
                        {
                            if (platforms == OsPlatforms.None)
                                platforms = next;
                            else
                                platforms |= next;
                        }

                        break;
                }
            }

            this.OsPlatforms = platforms;
        }

        public OsPlatforms OsPlatforms { get; set; }

        public override string? GetSkipReason(IMessageSink sink, ITestMethod testMethod, IAttributeInfo attributeInfo)
        {
            if (!DiscovererHelpers.TestPlatformApplies(this.OsPlatforms))
            {
                return $"Requires OS: {this.OsPlatforms}";
            }

            return null;
        }
    }
}