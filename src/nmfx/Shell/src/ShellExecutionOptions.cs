using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NerdyMishka.Shell.Abstractions;

namespace NerdyMishka.Shell
{
    public class ShellExecutionOptions : IShellExecutionOptions, IMutableShellExecutionOptions
    {
        public TimeSpan? TimeOut { get; set; }

        public string? WorkingDirectory { get; set; }

        public Encoding? Encoding { get; set; }

        public IReadOnlyDictionary<string, string>? EnvironmentVariables { get; set; }

        public CancellationToken? CancellationToken { get; set; }

        public IShellExecutionOptions Overwrite(IShellExecutionOptions other)
        {
            if (other.TimeOut.HasValue)
                this.TimeOut = other.TimeOut;

            if (!Check.IsNullOrWhiteSpace(other.WorkingDirectory))
                this.WorkingDirectory = other.WorkingDirectory;

            if (other.Encoding != null)
                this.Encoding = other.Encoding;

            if (other.CancellationToken.HasValue)
                this.CancellationToken = other.CancellationToken;

            if (this.EnvironmentVariables == null && other.EnvironmentVariables == null)
            {
                return this;
            }

            var variables = new Dictionary<string, string>();
            if (this.EnvironmentVariables != null)
            {
                foreach (var kv in this.EnvironmentVariables)
                    variables[kv.Key] = kv.Value;
            }

            if (other.EnvironmentVariables != null)
            {
                foreach (var kv in other.EnvironmentVariables)
                    variables[kv.Key] = kv.Value;
            }

            this.EnvironmentVariables = variables;
            return this;
        }

        public IShellExecutionOptions Mixin(IShellExecutionOptions other)
        {
            var clone = (ShellExecutionOptions)this.MemberwiseClone();
            return clone.Overwrite(other);
        }
    }
}
