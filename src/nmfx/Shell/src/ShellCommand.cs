using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Medallion.Shell;
using NerdyMishka.Shell.Abstractions;
using NerdyMishka.Util;
using MedallionShell = Medallion.Shell.Shell;
using Options = Medallion.Shell.Shell.Options;

namespace NerdyMishka.Shell
{
    internal class ShellCommand : IShellCommand
    {
        private readonly Medallion.Shell.Command command;

        private readonly IShellRequest request;

        private bool outputRedirected;

        private bool errorRedirected;

        private IShellResponse? lastResponse;

        public ShellCommand(IShellRequest request)
        {
            this.request = request;
            this.command = Command.Run(request.FileName, request.Arguments, ApplyOptions(request));
        }

        protected internal ShellCommand(IShellRequest request, MedallionShell shell)
        {
            this.request = request;
            this.command = shell.Run(request.FileName, request.Arguments, ApplyOptions(request));
        }

        public IShellCommand RedirectTo(TextWriter writer)
        {
            this.outputRedirected = true;
            this.command.RedirectTo(writer);
            return this;
        }

        public IShellCommand RedirectTo(Stream stream)
        {
            this.command.RedirectTo(stream);
            return this;
        }

        public IShellCommand RedirectTo(ICollection<string> collection)
        {
            this.outputRedirected = true;
            this.command.RedirectTo(collection);
            return this;
        }

        public IShellCommand RedirectErrorTo(TextWriter writer)
        {
            this.errorRedirected = true;
            this.command.RedirectStandardErrorTo(writer);
            return this;
        }

        public IShellCommand RedirectErrorTo(Stream stream)
        {
            this.errorRedirected = true;
            this.command.RedirectStandardErrorTo(stream);
            return this;
        }

        public IShellCommand RedirectErrorTo(ICollection<string> collection)
        {
            this.errorRedirected = true;
            this.command.RedirectStandardErrorTo(collection);
            return this;
        }

        public void Kill()
            => this.command.Kill();

        public void Wait()
            => this.command.Wait();

        public async Task WaitAsync()
        {
            if (this.lastResponse != null)
                return;

            var result = await this.command.Task.ConfigureAwait(false);
            this.lastResponse = this.ConvertToResponse(result);
        }

        public IShellResponse ToResponse()
        {
            if (this.lastResponse != null)
                return this.lastResponse;

            var result = this.command.Result;
            this.lastResponse = this.ConvertToResponse(result);
            return this.lastResponse;
        }

        public async Task<IShellResponse> ToResponseAsync()
        {
            if (this.lastResponse != null)
                return this.lastResponse;

            var result = await this.command.Task.ConfigureAwait(false);
            this.lastResponse = this.ConvertToResponse(result);
            return this.lastResponse;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(true);
        }

        internal static Action<Options> ApplyOptions(IShellExecutionOptions executeOptions)
        {
            return (o) =>
            {
                o.DisposeOnExit(false);

                if (executeOptions.Encoding != null)
                    o.Encoding(executeOptions.Encoding);

                if (executeOptions.TimeOut.HasValue)
                    o.Timeout(executeOptions.TimeOut.Value);

                if (!Check.IsNullOrWhiteSpace(executeOptions.WorkingDirectory))
                    o.WorkingDirectory(executeOptions.WorkingDirectory);

                if (executeOptions.EnvironmentVariables is { Count: > 0 })
                {
                    o.EnvironmentVariables(executeOptions.EnvironmentVariables);
                }

                if (executeOptions.CancellationToken != null)
                    o.CancellationToken(executeOptions.CancellationToken.Value);
            };
        }

        internal IShellResponse ConvertToResponse(CommandResult result)
        {
            IReadOnlyList<string>? output = null;
            IReadOnlyList<string>? error = null;

            if (!this.outputRedirected)
                output = new List<string>(this.command.StandardOutput.GetLines());

            if (!this.errorRedirected)
                error = new List<string>(this.command.StandardError.GetLines());

            var response = new ShellResponse(this.request.FileName, this.request.Arguments)
            {
                ExitCode = result.ExitCode,
                StandardOutput = output,
                StandardError = error,
                StartTime = this.command.Process.StartTime,
                EndTime = this.command.Process.ExitTime,
                WorkingDirectory = this.request.WorkingDirectory,
                Encoding = this.request.Encoding,
                CancellationToken = this.request.CancellationToken,
                EnvironmentVariables = this.request.EnvironmentVariables,
                TimeOut = this.request.TimeOut,
            };

            return response;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            this.command.Process.Dispose();

            if (this.command is IDisposable disposable)
                disposable.Dispose();
        }
    }
}
