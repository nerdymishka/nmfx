using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NerdyMishka.Shell.Abstractions
{
    public interface IShell : IShellExecutable
    {
        IShellResponse Capture(
            string fileName,
            IEnumerable<string>? arguments = null,
            IShellExecutionOptions? options = null);

        Task<IShellResponse> CaptureAsync(
            string fileName,
            IEnumerable<string>? arguments = null,
            IShellExecutionOptions? options = null,
            CancellationToken cancellationToken = default);

        IShellResponse Exec(
            string fileName,
            IEnumerable<string>? arguments = null,
            IShellExecutionOptions? options = null);

        Task<IShellResponse> ExecAsync(
            string fileName,
            IEnumerable<string>? arguments = null,
            IShellExecutionOptions? options = null,
            CancellationToken cancellationToken = default);

        IShellCommand Start(
            string fileName,
            IEnumerable<string>? arguments = null,
            IShellExecutionOptions? options = null);

        IShellResponse Tee(
            string fileName,
            TextWriter standardOutputWriter,
            TextWriter standardErrorWriter,
            IEnumerable<string>? arguments = null,
            IShellExecutionOptions? options = null);

        Task<IShellResponse> TeeAsync(
            string fileName,
            TextWriter standardOutputWriter,
            TextWriter standardErrorWriter,
            IEnumerable<string>? arguments = null,
            IShellExecutionOptions? options = null,
            CancellationToken cancellationToken = default);
    }
}
