// <copyright file="DotNetFileSystemEntry.cs" company="Fubar Development Junker">
// Copyright (c) Fubar Development Junker. All rights reserved.
// </copyright>

using System;
using System.IO;
using FubarDev.FtpServer.FileSystem;
using FubarDev.FtpServer.FileSystem.Generic;

namespace rFtpSrvFileSystem
{
    /// <summary>
    /// Base class for System.IO based file system entries.
    /// </summary>
    public abstract class FileSystemEntry : IUnixFileSystemEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemEntry"/> class.
        /// </summary>
        /// <param name="fsInfo">The <see cref="FileSystemInfo"/> to extract the information from.</param>
        protected FileSystemEntry(FileSystemInfo fsInfo)
        {
            Info = fsInfo;
            LastWriteTime = new DateTimeOffset(Info.LastWriteTime);
            CreatedTime = new DateTimeOffset(Info.CreationTimeUtc);

            var accessMode = fsInfo.FullName.EndsWith("block.txt", StringComparison.OrdinalIgnoreCase)
                ? new GenericAccessMode(false, false, false)
                : new GenericAccessMode(true, true, true);

            Permissions = new GenericUnixPermissions(accessMode, accessMode, accessMode);
        }

        /// <summary>
        /// Gets the underlying <see cref="DirectoryInfo"/>.
        /// </summary>
        public FileSystemInfo Info { get; }

        /// <inheritdoc/>
        public string Name => Info.Name;

        /// <inheritdoc/>
        public IUnixPermissions Permissions { get; }

        /// <inheritdoc/>
        public DateTimeOffset? LastWriteTime { get; }

        /// <inheritdoc/>
        public DateTimeOffset? CreatedTime { get; }

        /// <inheritdoc/>
        public long NumberOfLinks => 1;

        /// <inheritdoc/>
        public string Owner => "owner";

        /// <inheritdoc/>
        public string Group => "group";

        public bool IsBlocked => false; 
    }
}
