using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Mettle.Sdk;

namespace Mettle.Sdk
{
    internal static class AssertHelper
    {
        private static ConcurrentDictionary<Type, Dictionary<string, Func<object?, object?>>> gettersByType = new ConcurrentDictionary<Type, Dictionary<string, Func<object?, object?>>>();

        public static EquivalentException? VerifyEquivalence(
            object? expected,
            object? actual,
            bool strict)
        {
            return VerifyEquivalence(expected, actual, strict, string.Empty, new HashSet<object>(), new HashSet<object>());
        }

        private static Dictionary<string, Func<object?, object?>> GetGettersForType(Type type)
        {
            return gettersByType.GetOrAdd(type, typeInfo =>
            {
                var fieldGetters =
                    typeInfo
                        .GetRuntimeFields()
                        .Where(f => f.IsPublic && !f.IsStatic)
                        .Select(f => new { name = f.Name, getter = (Func<object?, object?>)f.GetValue });

                var propertyGetters =
                    typeInfo
                        .GetRuntimeProperties()
                        .Where(p => p.CanRead)
                        .Select(p => new { name = p.Name, getter = (Func<object?, object?>)p.GetValue });

                return
                    fieldGetters
                        .Concat(propertyGetters)
                        .ToDictionary(g => g.name, g => g.getter);
            });
        }

        private static EquivalentException? VerifyEquivalence(
            object? expected,
            object? actual,
            bool strict,
            string prefix,
            HashSet<object> expectedRefs,
            HashSet<object> actualRefs)
        {
            // Check for null equivalence
            if (expected == null)
            {
                return
                    actual == null
                        ? null
                        : EquivalentException.ForMemberValueMismatch(expected, actual, prefix);
            }

            if (actual == null)
                return EquivalentException.ForMemberValueMismatch(expected, actual, prefix);

            // Check for identical references
            if (object.ReferenceEquals(expected, actual))
                return null;

            // Prevent circular references
            if (expectedRefs.Contains(expected))
                return EquivalentException.ForCircularReference($"{nameof(expected)}.{prefix}");

            if (actualRefs.Contains(actual))
                return EquivalentException.ForCircularReference($"{nameof(actual)}.{prefix}");

            expectedRefs.Add(expected);
            actualRefs.Add(actual);

            try
            {
                // Value types and strings should just fall back to their Equals implementation
                var expectedType = expected.GetType();
                if (expectedType.GetTypeInfo().IsValueType || expectedType == typeof(string))
                {
                    return
                        expected.Equals(actual)
                            ? null
                            : EquivalentException.ForMemberValueMismatch(expected, actual, prefix);
                }

                // Enumerables? Check equivalence of individual members
                var enumerableExpected = expected as IEnumerable;
                var enumerableActual = actual as IEnumerable;
                if (enumerableExpected != null && enumerableActual != null)
                    return VerifyEquivalenceEnumerable(enumerableExpected, enumerableActual, strict, prefix, expectedRefs, actualRefs);

                return VerifyEquivalenceReference(expected, actual, strict, prefix, expectedRefs, actualRefs);
            }
            finally
            {
                expectedRefs.Remove(expected);
                actualRefs.Remove(actual);
            }
        }

        private static EquivalentException? VerifyEquivalenceEnumerable(
            IEnumerable expected,
            IEnumerable actual,
            bool strict,
            string prefix,
            HashSet<object> expectedRefs,
            HashSet<object> actualRefs)
        {
            var expectedValues = expected.Cast<object?>().ToList();
            var actualValues = actual.Cast<object?>().ToList();
            var actualOriginalValues = actualValues.ToList();

            // Walk the list of expected values, and look for actual values that are equivalent
            foreach (var expectedValue in expectedValues)
            {
                var actualIdx = 0;
                for (; actualIdx < actualValues.Count; ++actualIdx)
                {
                    if (VerifyEquivalence(expectedValue, actualValues[actualIdx], strict, string.Empty, expectedRefs, actualRefs) == null)
                        break;
                }

                if (actualIdx == actualValues.Count)
                    return EquivalentException.ForMissingCollectionValue(expectedValue, actualOriginalValues, prefix);

                actualValues.RemoveAt(actualIdx);
            }

            if (strict && actualValues.Count != 0)
                return EquivalentException.ForExtraCollectionValue(expectedValues, actualOriginalValues, actualValues, prefix);

            return null;
        }

        private static EquivalentException? VerifyEquivalenceReference(
            object expected,
            object actual,
            bool strict,
            string prefix,
            HashSet<object> expectedRefs,
            HashSet<object> actualRefs)
        {
            var prefixDot = prefix == string.Empty ? string.Empty : prefix + ".";

            // Enumerate over public instance fields and properties and validate equivalence
            var expectedGetters = GetGettersForType(expected.GetType());
            var actualGetters = GetGettersForType(actual.GetType());

            if (strict && expectedGetters.Count != actualGetters.Count)
                return EquivalentException.ForMemberListMismatch(expectedGetters.Keys, actualGetters.Keys, prefixDot);

            foreach (var kvp in expectedGetters)
            {
                Func<object?, object?>? actualGetter;

                if (!actualGetters.TryGetValue(kvp.Key, out actualGetter))
                    return EquivalentException.ForMemberListMismatch(expectedGetters.Keys, actualGetters.Keys, prefixDot);

                var expectedMemberValue = kvp.Value(expected);
                var actualMemberValue = actualGetter(actual);

                var ex = VerifyEquivalence(expectedMemberValue, actualMemberValue, strict, prefixDot + kvp.Key, expectedRefs, actualRefs);
                if (ex != null)
                    return ex;
            }

            return null;
        }
    }
}