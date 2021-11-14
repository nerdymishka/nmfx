using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NerdyMishka.Shell.Abstractions;

namespace NerdyMishka.Shell
{
    public class ShellRequest : ShellExecutionOptions, IShellRequest
    {
        private static readonly List<string> WindowsExtensions = new()
        {
            ".exe",
            ".bat",
            ".cmd",
        };

        public ShellRequest(IShellRequest request)
        {
            this.FileName = request.FileName;
            this.Arguments = request.Arguments;
            this.EnvironmentVariables = request.EnvironmentVariables;
            this.Encoding = request.Encoding;
            this.TimeOut = request.TimeOut;
            this.WorkingDirectory = request.WorkingDirectory;
            this.CancellationToken = request.CancellationToken;
        }

        public ShellRequest(string fileName, IEnumerable<string>? arguments, IShellExecutionOptions? options = null)
        {
            if (!Shell.IsWindows)
            {
                var ext = Path.GetExtension(fileName);
                if (WindowsExtensions.Contains(ext))
                    fileName = Path.GetFileNameWithoutExtension(fileName);
            }

            this.FileName = fileName;
            if (arguments == null)
            {
                this.Arguments = Array.Empty<string>();
                return;
            }

            this.Arguments = new List<string>(arguments);

            if (options is null)
                return;

            this.Overwrite(options);
        }

        public string FileName { get; }

        public IReadOnlyList<string> Arguments { get; }
    }
}
