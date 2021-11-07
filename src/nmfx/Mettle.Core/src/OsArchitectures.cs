using System;

namespace Mettle
{
    [Flags]
    public enum OsArchitectures
    {
        None = 0,

        X86 = 1,

        X64 = 2,

        Arm = 4,

        Arm64 = 8,
    }
}