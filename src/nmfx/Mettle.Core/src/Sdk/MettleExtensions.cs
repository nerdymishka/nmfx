using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mettle.Sdk
{
    public static class MettleExtensions
    {
        public static void Add<TKey, TValue>(
            this IDictionary<TKey, List<TValue>> dictionary,
            TKey key,
            TValue value)
        {
            dictionary.GetOrAdd(key).Add(value);
        }

        public static TValue GetOrAdd<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key)
                where TValue : new()
        {
            return dictionary.GetOrAdd<TKey, TValue>(key, () => new TValue());
        }

        public static TValue GetOrAdd<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> newValue)
        {
            if (dictionary.TryGetValue(key, out var result))
                return result;

            result = newValue();
            dictionary[key] = result;

            return result;
        }

        internal static object? GetDefaultValue(this TypeInfo typeInfo)
        {
            if (typeInfo.IsValueType)
                return Activator.CreateInstance(typeInfo.AsType());

            return null;
        }
    }
}