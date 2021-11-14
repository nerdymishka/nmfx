using System;
using System.Collections.Generic;
using System.Text;
using NerdyMishka.Shell.Abstractions;

namespace NerdyMishka.Shell.Abstractions
{
    public interface IShellRequest : IShellExecutionOptions
    {
        string FileName { get; }

        IReadOnlyList<string> Arguments { get; }
    }
}
