using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Shell.Abstractions
{
    public interface IShellCommand : IDisposable
    {
        IShellCommand RedirectTo(TextWriter writer);

        IShellCommand RedirectTo(Stream stream);

        IShellCommand RedirectTo(ICollection<string> collection);

        IShellCommand RedirectErrorTo(TextWriter writer);

        IShellCommand RedirectErrorTo(Stream stream);

        IShellCommand RedirectErrorTo(ICollection<string> collection);

        void Kill();

        void Wait();

        Task WaitAsync();

        IShellResponse ToResponse();

        Task<IShellResponse> ToResponseAsync();
    }
}
