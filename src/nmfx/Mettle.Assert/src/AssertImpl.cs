using System;
using System.Collections;
using System.Collections.Generic;
using Mettle.Sdk;

namespace Mettle
{
    public partial class AssertImpl : IAssert
    {
        public static AssertImpl Instance { get; } = new AssertImpl();

        private static IComparer<T> GetComparer<T>()
            where T : IComparable
        {
            return new AssertComparer<T>();
        }

        private static IEqualityComparer<T?> GetEqualityComparer<T>(IEqualityComparer? innerComparer = null) =>
            new AssertEqualityComparer<T?>(innerComparer);
    }
}