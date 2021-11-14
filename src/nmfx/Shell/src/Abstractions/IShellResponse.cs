using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Shell.Abstractions
{
    public interface IShellResponse : IShellExecutionOptions
    {
        string FileName { get; }

        IReadOnlyList<string> Arguments { get; }

        int ExitCode { get; }

        IReadOnlyList<string>? StandardOutput { get; }

        IReadOnlyList<string>? StandardError { get; }

        DateTimeOffset StartTime { get; }

        DateTimeOffset EndTime { get; }
    }
}
