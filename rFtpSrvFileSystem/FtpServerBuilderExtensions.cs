// <copyright file="DotNetFtpServerBuilderExtensions.cs" company="Fubar Development Junker">
// Copyright (c) Fubar Development Junker. All rights reserved.
// </copyright>

using FubarDev.FtpServer.FileSystem;
using rFtpSrvFileSystem;

using Microsoft.Extensions.DependencyInjection;
using FubarDev.FtpServer;

// ReSharper disable once CheckNamespace
namespace rFtpSrvCore.FileSystem
{
    /// <summary>
    /// Extension methods for <see cref="IFtpServerBuilder"/>.
    /// </summary>
    public static class DotNetFtpServerBuilderExtensions
    {
        /// <summary>
        /// Uses the .NET file system API.
        /// </summary>
        /// <param name="builder">The server builder used to configure the FTP server.</param>
        /// <returns>the server builder used to configure the FTP server.</returns>
        public static IFtpServerBuilder UseCustomFileSystem(this IFtpServerBuilder builder)
        {
            builder.Services.AddSingleton<IFileSystemClassFactory, FileSystemProvider>();
            return builder;
        }
    }
}
