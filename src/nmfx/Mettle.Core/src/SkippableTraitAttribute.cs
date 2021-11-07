using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace Mettle
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public abstract class SkippableTraitAttribute : Attribute
    {
        private static readonly ConcurrentDictionary<string, Type> Traits = new();

        internal static IReadOnlyDictionary<string, Type> SkippableTraits => Traits;

        public static void AddTraitAttribute(Type type)
        {
            if (!Traits.ContainsKey(type.FullName))
                Traits.TryAdd(type.FullName, type);
        }

        public abstract string? GetSkipReason(IMessageSink sink, ITestMethod testMethod, IAttributeInfo attributeInfo);
    }
}