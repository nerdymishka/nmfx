using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mettle;
using NerdyMishka.Shell;
using NerdyMishka.Shell.Abstractions;
using Xunit.Abstractions;
using static NerdyMishka.Util.EnumerableMethods;

[assembly:MettleTestFramework]

namespace Tests
{
    public static class ShellTests
    {
        private static readonly ShellRequest BadCommand = new(
            "dotnet.exe",
            FromParams("iHazCheeze"));

        private static readonly ShellRequest VersionCommand = new(
            "dotnet.exe",
            FromParams("--version"));

        [UnitTest]
        public static void VerifyExec(IAssert assert)
        {
            var result = Shell.Exec(VersionCommand);
            assert.NotNull(result);
            assert.Equal(0, result.ExitCode);
        }

        [UnitTest]
        public static void VerifyExecErrorStream(IAssert assert)
        {
            var result = Shell.Exec(BadCommand);
            assert.NotNull(result);
            assert.NotNull(result.StandardError);
            assert.True(result.StandardError.Count > 3);
        }

        [UnitTest]
        public static void VerifyCapture(IAssert assert)
        {
            var result = Shell.Capture(VersionCommand);
            assert.NotNull(result);
            assert.Equal(0, result.ExitCode);
            assert.NotNull(result.StandardOutput);
            assert.Equal(1, result.StandardOutput.Count);

            if (!Version.TryParse(result.StandardOutput[0], out var version))
            {
                assert.Fail($"Unable to parse dotnet version {result.StandardOutput[0]}");
                return;
            }

            assert.True(version.Major > 0);
        }

        [UnitTest]
        public static void VerifyTee(IAssert assert)
        {
            using var sw = new StringWriter();
            var result = Shell.Tee(VersionCommand, sw, sw);
            assert.NotNull(result);
            var output = sw.ToString();
            assert.NotNull(output);

            if (!Version.TryParse(output, out var version))
            {
                assert.Fail($"unable to parse version from {output}");
                return;
            }

            assert.True(version.Major > 0);
        }
    }
}