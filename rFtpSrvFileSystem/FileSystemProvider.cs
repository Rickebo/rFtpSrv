//-----------------------------------------------------------------------
// <copyright file="DotNetFileSystemProvider.cs" company="Fubar Development Junker">
//     Copyright (c) Fubar Development Junker. All rights reserved.
// </copyright>
// <author>Mark Junker</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FubarDev.FtpServer;
using FubarDev.FtpServer.FileSystem;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace rFtpSrvFileSystem
{
    /// <summary>
    /// A <see cref="IFileSystemClassFactory"/> implementation that uses
    /// the standard .NET functionality to provide file system access.
    /// </summary>
    public class FileSystemProvider : IFileSystemClassFactory
    {
        private readonly IAccountDirectoryQuery _accountDirectoryQuery;
        #nullable enable
        private readonly ILogger<FileSystemProvider>? _logger;
        private readonly string _rootPath;
        private readonly int _streamBufferSize;
        private readonly bool _allowNonEmptyDirectoryDelete;
        private readonly bool _flushAfterWrite;
        private readonly BlockCollection _blocks;
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemProvider"/> class.
        /// </summary>
        /// <param name="options">The file system options.</param>
        /// <param name="accountDirectoryQuery">Interface to query account directories.</param>
        /// <param name="logger">The logger.</param>
        #nullable enable
        public FileSystemProvider(
            IOptions<FileSystemOptions> options,
            IAccountDirectoryQuery accountDirectoryQuery,
            ILogger<FileSystemProvider>? logger = null)
        {
            _accountDirectoryQuery = accountDirectoryQuery;
            _logger = logger;
            _rootPath = string.IsNullOrEmpty(options.Value.RootPath)
                ? Path.GetTempPath()
                : options.Value.RootPath!;
            _streamBufferSize = options.Value.StreamBufferSize ?? FileSystem.DefaultStreamBufferSize;
            _allowNonEmptyDirectoryDelete = options.Value.AllowNonEmptyDirectoryDelete;
            _flushAfterWrite = options.Value.FlushAfterWrite;
            _blocks = options.Value.Blocks;
        }

        /// <inheritdoc/>
        public Task<IUnixFileSystem> Create(IAccountInformation accountInformation)
        {
            var path = _rootPath;
            var directories = _accountDirectoryQuery.GetDirectories(accountInformation);
            if (!string.IsNullOrEmpty(directories.RootPath))
            {
                path = Path.Combine(path, directories.RootPath);
            }

            _logger?.LogDebug("The root directory for {userName} is {rootPath}", accountInformation.FtpUser.Identity.Name, path);

            return Task.FromResult<IUnixFileSystem>(new FileSystem(path, _allowNonEmptyDirectoryDelete, _streamBufferSize, _flushAfterWrite, _blocks));
        }
    }
}
