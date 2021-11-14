using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Shell.Abstractions
{
    public interface IMutableShellResponse : IMutableShellExecutionOptions
    {
        string FileName { get; set; }

        IReadOnlyList<string> Arguments { get; set; }

        int ExitCode { get; set; }

        IReadOnlyList<string>? StandardOutput { get; set; }

        IReadOnlyList<string>? StandardError { get; set; }

        DateTimeOffset StartTime { get; set; }

        DateTimeOffset EndTime { get; set; }
    }
}
