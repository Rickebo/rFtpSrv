using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace rFtpSrvFileSystem
{
    public class FileSystemInfoWrapper
    {
        public static readonly Func<string, string> StringFormatter = str => str.ToLowerInvariant();

        public readonly FileSystemInfo Info;
        public readonly string RootedPath;

        public FileSystemInfoWrapper(FileSystemInfo info)
        {
            Info = info;
            RootedPath = StringFormatter(Path.GetFullPath(info.FullName));
        }

        public override bool Equals(object obj)
        {
            if (obj is FileSystemInfoWrapper other)
                return RootedPath == other.RootedPath;

            return false;
        }

        public override int GetHashCode() => RootedPath.GetHashCode();
    }
}
