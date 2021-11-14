using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NerdyMishka.Shell.Abstractions;

namespace NerdyMishka.Shell
{
    internal class ShellResponse : IShellResponse, IMutableShellResponse
    {
        public ShellResponse(string fileName, IEnumerable<string>? arguments)
        {
            this.FileName = fileName;
            if (arguments == null)
            {
                this.Arguments = Array.Empty<string>();
                return;
            }

            this.Arguments = new List<string>(arguments);
        }

        public TimeSpan? TimeOut { get; set; }

        public string? WorkingDirectory { get; set; }

        public Encoding? Encoding { get; set; }

        public IReadOnlyDictionary<string, string>? EnvironmentVariables { get; set; }

        public CancellationToken? CancellationToken { get; set; }

        public string FileName { get; set; }

        public IReadOnlyList<string> Arguments { get; set; }

        public int ExitCode { get; set; }

        public IReadOnlyList<string>? StandardOutput { get; set; }

        public IReadOnlyList<string>? StandardError { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }
    }
}
