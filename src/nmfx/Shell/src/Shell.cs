using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NerdyMishka.IO;
using NerdyMishka.Shell.Abstractions;
using NerdyMishka.Util;
using MedallionShell = Medallion.Shell.Shell;

// ReSharper disable once ParameterHidesMember
namespace NerdyMishka.Shell
{
    public class Shell : IShell
    {
        private readonly MedallionShell shell;

        private readonly string fileName;

        public Shell(string fileName, IShellExecutionOptions? executionOptions)
            : this(executionOptions)
        {
            this.fileName = fileName;
        }

        public Shell(IShellExecutionOptions? executionOptions)
        {
            if (IsWindows)
            {
                this.fileName = "powershell.exe";
            }
            else if (IsMacOsx)
            {
                this.fileName = "zsh";
            }
            else
            {
                this.fileName = "bash";
            }

            if (executionOptions != null)
            {
                this.shell = new MedallionShell(ShellCommand.ApplyOptions(executionOptions));
                return;
            }

            this.shell = new MedallionShell(_ => { });
        }

        public static bool IsWindows { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool IsLinux { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public static bool IsMacOsx { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static IShell Create(IShellExecutionOptions? options)
        {
            return new Shell(options);
        }

        public static IShell Create(string fileName, IShellExecutionOptions? options)
        {
            return new Shell(fileName, options);
        }

        public static IShellResponse Capture(IShellRequest request)
        {
            var cmd = Start(request);
            return cmd.ToResponse();
        }

        public static Task<IShellResponse> CaptureAsync(
            IShellRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request.CancellationToken == null)
                request = new ShellRequest(request) { CancellationToken = cancellationToken };

            var cmd = Start(request);
            return cmd.ToResponseAsync();
        }

        public static IShellResponse Exec(IShellRequest request)
        {
            using var cmd = Start(request);
            using var error = new TeeTextWriter(new StringWriter(), Console.Error, true);
            cmd.RedirectErrorTo(error);
            cmd.RedirectTo(Console.Out);

            var response = cmd.ToResponse();
            if (response is IMutableShellResponse mutable)
            {
                mutable.StandardError = error.ToString().ToLines();
            }

            return response;
        }

        public static Task<IShellResponse> ExecAsync(
            string fileName,
            IEnumerable<string>? arguments = null,
            IShellExecutionOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            return ExecAsync(
                new ShellRequest(fileName, arguments, options),
                cancellationToken);
        }

        public static async Task<IShellResponse> ExecAsync(
            IShellRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request.CancellationToken == null)
                request = new ShellRequest(request) { CancellationToken = cancellationToken };

            using var cmd = Start(request);
            using var error = new TeeTextWriter(new StringWriter(), Console.Error, true);
            cmd.RedirectErrorTo(error);
            cmd.RedirectTo(Console.Out);

            var response = await cmd.ToResponseAsync()
                .ConfigureAwait(false);
            if (response is IMutableShellResponse mutable)
            {
                mutable.StandardError = error.ToString().ToLines();
            }

            return response;
        }

        public static IShellCommand Start(string fileName, params string[] arguments)
            => new ShellCommand(new ShellRequest(fileName, arguments ?? Array.Empty<string>()));

        public static IShellCommand Start(string fileName, IEnumerable<string>? arguments = null, IShellExecutionOptions? options = null)
            => new ShellCommand(new ShellRequest(fileName, arguments, options));

        public static IShellCommand Start(IShellRequest request)
            => new ShellCommand(request);

        public static IShellResponse Tee(IShellRequest request, TextWriter standardOutputWriter, TextWriter standardErrorWriter)
        {
            using var cmd = Start(request);
            using var output = new TeeTextWriter(standardOutputWriter, Console.Out, false);
            using var error = new TeeTextWriter(standardErrorWriter, Console.Error, false);

            cmd.RedirectErrorTo(error);
            cmd.RedirectTo(output);

            return cmd.ToResponse();
        }

        public static async Task<IShellResponse> TeeAsync(
            IShellRequest request,
            TextWriter standardOutputWriter,
            TextWriter standardErrorWriter,
            CancellationToken cancellationToken = default)
        {
            if (request.CancellationToken == null)
                request = new ShellRequest(request) { CancellationToken = cancellationToken };

            using var cmd = Start(request);
            using var output = new TeeTextWriter(standardOutputWriter, Console.Out, false);
            using var error = new TeeTextWriter(standardErrorWriter, Console.Error, false);

            cmd.RedirectErrorTo(error);
            cmd.RedirectTo(output);

            return await cmd.ToResponseAsync().ConfigureAwait(false);
        }

        IShellResponse IShellExecutable.Capture(IShellRequest request)
            => Capture(request, this);

        IShellResponse IShell.Capture(
            string fileName,
            IEnumerable<string>? arguments,
            IShellExecutionOptions? options)
            => Capture(new ShellRequest(fileName, arguments, options), this);

        Task<IShellResponse> IShellExecutable.CaptureAsync(
            IShellRequest request,
            CancellationToken cancellationToken)
            => CaptureAsync(request, this, cancellationToken);

        Task<IShellResponse> IShell.CaptureAsync(
            string fileName,
            IEnumerable<string>? arguments,
            IShellExecutionOptions? options,
            CancellationToken cancellationToken)
            => CaptureAsync(new ShellRequest(fileName, arguments, options), this, cancellationToken);

        IShellResponse IShell.Exec(
            string fileName,
            IEnumerable<string>? arguments,
            IShellExecutionOptions? options)
            => Exec(new ShellRequest(fileName, arguments, options), this);

        IShellResponse IShellExecutable.Exec(IShellRequest request)
            => Exec(request, this);

        Task<IShellResponse> IShell.ExecAsync(
            string fileName,
            IEnumerable<string>? arguments,
            IShellExecutionOptions? options,
            CancellationToken cancellationToken)
            => ExecAsync(new ShellRequest(fileName, arguments, options), this, cancellationToken);

        Task<IShellResponse> IShellExecutable.ExecAsync(
            IShellRequest request,
            CancellationToken cancellationToken)
            => ExecAsync(request, this, cancellationToken);

        IShellCommand IShellExecutable.Start(IShellRequest request)
            => Start(request, this);

        IShellCommand IShell.Start(
            string? fileName,
            IEnumerable<string>? arguments,
            IShellExecutionOptions? options)
            => Start(new ShellRequest(fileName ?? this.fileName, arguments, options), this);

        IShellResponse IShell.Tee(
            string fileName,
            TextWriter standardOutputWriter,
            TextWriter standardErrorWriter,
            IEnumerable<string>? arguments,
            IShellExecutionOptions? options)
            => Tee(
                new ShellRequest(fileName, arguments, options),
                standardOutputWriter,
                standardErrorWriter,
                this);

        IShellResponse IShellExecutable.Tee(
            IShellRequest request,
            TextWriter standardOutputWriter,
            TextWriter standardErrorWriter)
            => Tee(request, standardOutputWriter, standardErrorWriter, this);

        Task<IShellResponse> IShell.TeeAsync(
            string fileName,
            TextWriter standardOutputWriter,
            TextWriter standardErrorWriter,
            IEnumerable<string>? arguments,
            IShellExecutionOptions? options,
            CancellationToken cancellationToken)
            => TeeAsync(
                new ShellRequest(fileName, arguments, options),
                standardOutputWriter,
                standardErrorWriter,
                cancellationToken);

        Task<IShellResponse> IShellExecutable.TeeAsync(
            IShellRequest request,
            TextWriter standardOutputWriter,
            TextWriter standardErrorWriter,
            CancellationToken cancellationToken)
            => TeeAsync(request, standardOutputWriter, standardErrorWriter, this, cancellationToken);

        internal static IShellResponse Capture(IShellRequest request, Shell instance)
        {
            using var cmd = Start(request, instance);
            return cmd.ToResponse();
        }

        internal static async Task<IShellResponse> CaptureAsync(
            IShellRequest request,
            Shell instance,
            CancellationToken cancellationToken = default)
        {
            if (request.CancellationToken == null)
                request = new ShellRequest(request) { CancellationToken = cancellationToken };

            using var cmd = Start(request, instance);
            return await cmd.ToResponseAsync().ConfigureAwait(false);
        }

        internal static IShellResponse Exec(IShellRequest request, Shell instance)
        {
            using var cmd = Start(request, instance);
            using var error = new TeeTextWriter(new StringWriter(), Console.Error, true);
            cmd.RedirectErrorTo(error);
            cmd.RedirectTo(Console.Out);

            var response = cmd.ToResponse();
            if (response is IMutableShellResponse mutable)
            {
                mutable.StandardError = error.ToString().ToLines();
            }

            return response;
        }

        internal static async Task<IShellResponse> ExecAsync(
            IShellRequest request,
            Shell instance,
            CancellationToken cancellationToken = default)
        {
            if (request.CancellationToken == null)
                request = new ShellRequest(request) { CancellationToken = cancellationToken };

            using var cmd = Start(request, instance);
            using var error = new TeeTextWriter(new StringWriter(), Console.Error, true);
            cmd.RedirectErrorTo(error);
            cmd.RedirectTo(Console.Out);

            var response = await cmd.ToResponseAsync().ConfigureAwait(false);
            if (response is IMutableShellResponse mutable)
            {
                mutable.StandardError = error.ToString().ToLines();
            }

            return response;
        }

        internal static IShellCommand Start(IShellRequest request, Shell instance)
        {
            if (Check.IsNullOrWhiteSpace(request.FileName))
                request = new ShellRequest(instance.fileName, request.Arguments, request);

            return new ShellCommand(request, instance.shell);
        }

        internal static IShellResponse Tee(
            IShellRequest request,
            TextWriter standardOutputWriter,
            TextWriter standardErrorWriter,
            Shell instance)
        {
            using var cmd = Start(request, instance);
            using var output = new TeeTextWriter(standardOutputWriter, Console.Out, false);
            using var error = new TeeTextWriter(standardErrorWriter, Console.Error, false);

            cmd.RedirectErrorTo(error);
            cmd.RedirectTo(output);

            return cmd.ToResponse();
        }

        [SuppressMessage(
            "AsyncUsage",
            "AsyncFixer01:Unnecessary async/await usage",
            Justification = "ShellCommand may require async call")]
        internal static async Task<IShellResponse> TeeAsync(
            IShellRequest request,
            TextWriter standardOutputWriter,
            TextWriter standardErrorWriter,
            Shell instance,
            CancellationToken cancellationToken = default)
        {
            if (request.CancellationToken == null)
                request = new ShellRequest(request) { CancellationToken = cancellationToken };

            var cmd = Start(request, instance);
            using var output = new TeeTextWriter(standardOutputWriter, Console.Out, false);
            using var error = new TeeTextWriter(standardErrorWriter, Console.Error, false);

            cmd.RedirectErrorTo(error);
            cmd.RedirectTo(output);

            return await cmd.ToResponseAsync().ConfigureAwait(false);
        }
    }
}
