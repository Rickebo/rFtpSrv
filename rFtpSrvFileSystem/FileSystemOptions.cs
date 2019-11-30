// <copyright file="DotNetFileSystemOptions.cs" company="Fubar Development Junker">
// Copyright (c) Fubar Development Junker. All rights reserved.
// </copyright>

namespace rFtpSrvFileSystem
{
    /// <summary>
    /// Options for the .NET API based file system access.
    /// </summary>
    public class FileSystemOptions
    {
        /// <summary>
        /// Gets or sets the root path for all users.
        /// </summary>
        #nullable enable
        public string? RootPath { get; set; }

        public BlockCollection Blocks { get; set; } = new BlockCollection();

        /// <summary>
        /// Gets or sets the buffer size to be used in async IO methods.
        /// </summary>
        public int? StreamBufferSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether deletion of non-empty directories is allowed.
        /// </summary>
        public bool AllowNonEmptyDirectoryDelete { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the content should be flushed to disk after every write operation.
        /// </summary>
        public bool FlushAfterWrite { get; set; }
    }
}
