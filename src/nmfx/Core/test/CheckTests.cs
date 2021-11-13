using System;
using System.Collections.Generic;
using System.Text;
using Mettle;

[assembly:MettleTestFramework]

namespace NerdyMishka.Core.Tests
{
    public static class CheckTests
    {
        [UnitTest(DisplayName = "NotNull returns non null value.")]
        public static void NotNullReturnNonNullValue(IAssert assert)
        {
            var obj = new object();
            var result = Check.NotNull(obj, nameof(obj));
            assert.NotNull(result);
        }

        [UnitTest(DisplayName = "NotNull throws when null")]
        public static void NotNullThrowsWhenNull(IAssert assert)
        {
            assert.Throws<ArgumentNullException>(() =>
            {
                object? obj = null;
                Check.NotNull(obj, nameof(obj));
            });
        }

        [UnitTest]
        public static void NotNullOrEmptyThrowsWhenEmpty(IAssert assert)
        {
            assert.Throws<ArgumentNullException>(() =>
            {
                var empty = string.Empty;
                Check.NotNullOrEmpty(empty, nameof(empty));
            });
        }

        [UnitTest]
        public static void NotNullOrEmptyThrowsWhenNull(IAssert assert)
        {
            assert.Throws<ArgumentNullException>(() =>
            {
                const string? empty = null;
                Check.NotNullOrEmpty(empty, nameof(empty));
            });
        }

        [UnitTest]
        public static void NotNullOrWhiteSpaceThrowsWhenWhiteSpace(IAssert assert)
        {
            assert.Throws<ArgumentNullException>(() =>
            {
                const string? empty = "    ";
                Check.NotNullOrWhiteSpace(empty, nameof(empty));
            });
        }
    }
}
