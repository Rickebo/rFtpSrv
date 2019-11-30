using System;
using System.Collections.Generic;
using System.Text;
using FubarDev.FtpServer;
using FubarDev.FtpServer.AccountManagement;
using Microsoft.Extensions.DependencyInjection;

namespace rFtpSrvFileSystem
{
    public static class CustomFtpServerBuilderExtensions
    {

        /// <summary>
        /// Enabled custom authentication.
        /// </summary>
        /// <param name="builder">The server builder used to configure the FTP server.</param>
        /// <returns>the server builder used to configure the FTP server.</returns>
        public static IFtpServerBuilder EnableCustomAuthentication(this IFtpServerBuilder builder)
        {
            builder.Services.AddSingleton<IMembershipProvider, MembershipProvider>();
            return builder;
        }
    }
}
