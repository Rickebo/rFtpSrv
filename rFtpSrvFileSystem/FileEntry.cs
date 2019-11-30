//-----------------------------------------------------------------------
// <copyright file="DotNetFileEntry.cs" company="Fubar Development Junker">
//     Copyright (c) Fubar Development Junker. All rights reserved.
// </copyright>
// <author>Mark Junker</author>
//-----------------------------------------------------------------------

using FubarDev.FtpServer.FileSystem;
using System.IO;

namespace rFtpSrvFileSystem
{
    /// <summary>
    /// A <see cref="IUnixFileEntry"/> implementation for the standard
    /// .NET file system functionality.
    /// </summary>
    public class FileEntry : FileSystemEntry, IUnixFileEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileEntry"/> class.
        /// </summary>
        /// <param name="info">The <see cref="FileInfo"/> to extract the information from.</param>
        public FileEntry(FileInfo info)
            : base(info)
        {
            FileInfo = info;
        }

        /// <summary>
        /// Gets the file information.
        /// </summary>
        public FileInfo FileInfo { get; }

        /// <inheritdoc/>
        public long Size => FileInfo.Length;
    }
}
