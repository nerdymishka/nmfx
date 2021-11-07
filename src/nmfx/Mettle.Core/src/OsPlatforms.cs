using System;
using System.Diagnostics.CodeAnalysis;

namespace Mettle
{
    [Flags]
    public enum OsPlatforms
    {
        None = 0,
        Windows = 1,
        Linux = 2,
        OSX = 4,
        FreeBSD = 8,
        NetBSD = 16,
        Illumos = 32,
        Solaris = 64,
        IOS = 128,
        TVOS = 256,
        Android = 512,
        Browser = 1024,
        MacCatalyst = 2048,
        AnyUnix = FreeBSD | Linux | NetBSD | OSX | Illumos | Solaris | IOS | TVOS | MacCatalyst | Android | Browser,
    }
}