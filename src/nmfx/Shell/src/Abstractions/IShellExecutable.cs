using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NerdyMishka.Shell.Abstractions
{
    public interface IShellExecutable
    {
        IShellResponse Capture(IShellRequest request);

        Task<IShellResponse> CaptureAsync(
            IShellRequest request,
            CancellationToken cancellationToken = default);

        IShellResponse Exec(IShellRequest request);

        Task<IShellResponse> ExecAsync(
            IShellRequest request,
            CancellationToken cancellationToken = default);

        IShellCommand Start(IShellRequest request);

        IShellResponse Tee(
            IShellRequest request,
            TextWriter standardOutputWriter,
            TextWriter standardErrorWriter);

        Task<IShellResponse> TeeAsync(
            IShellRequest request,
            TextWriter standardOutputWriter,
            TextWriter standardErrorWriter,
            CancellationToken cancellationToken = default);
    }
}
