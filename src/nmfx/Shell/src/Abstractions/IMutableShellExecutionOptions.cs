using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NerdyMishka.Shell.Abstractions
{
    public interface IMutableShellExecutionOptions
    {
        TimeSpan? TimeOut { get; set; }

        string? WorkingDirectory { get; set; }

        Encoding? Encoding { get; set; }

        IReadOnlyDictionary<string, string>? EnvironmentVariables { get; set; }

        CancellationToken? CancellationToken { get; set; }
    }
}
