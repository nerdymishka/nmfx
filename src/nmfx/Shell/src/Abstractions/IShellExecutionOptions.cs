using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NerdyMishka.Shell.Abstractions
{
    public interface IShellExecutionOptions
    {
        TimeSpan? TimeOut { get; }

        string? WorkingDirectory { get; }

        Encoding? Encoding { get; }

        IReadOnlyDictionary<string, string>? EnvironmentVariables { get; }

        CancellationToken? CancellationToken { get; }
    }
}
