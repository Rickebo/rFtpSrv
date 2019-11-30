using FubarDev.FtpServer;
using Microsoft.Extensions.DependencyInjection;
using rFtpSrvCore.FileSystem;
using rFtpSrvFileSystem;
using System;
using System.IO;
using System.Threading;

namespace rFtpSrvCore
{
    class Program
    {
        static void Main(string[] args)
        {
            // Setup dependency injection
            var services = new ServiceCollection();
            string basePath = "TestDir";

            // use %TEMP%/TestFtpServer as root folder
            services.Configure<FileSystemOptions>(opt => opt
                .RootPath = Path.Combine(Environment.CurrentDirectory, basePath));

            services.Configure<FileSystemOptions>(opt => opt
                .Blocks.AddBlock(GetFileSystemObject(basePath, "block.txt"), BlockOperation.All));

            // Add FTP server services
            // DotNetFileSystemProvider = Use the .NET file system functionality
            // AnonymousMembershipProvider = allow only anonymous logins
            services.AddFtpServer(builder => builder
                .UseCustomFileSystem() // Use the .NET file system functionality
                .EnableAnonymousAuthentication()); // allow anonymous logins

            // Configure the FTP server
            services.Configure<FtpServerOptions>(opt => opt.ServerAddress = "127.0.0.1");

            // Build the service provider
            using (var serviceProvider = services.BuildServiceProvider())
            {
                // Initialize the FTP server
                var ftpServerHost = serviceProvider.GetRequiredService<IFtpServerHost>();

                // Start the FTP server
                ftpServerHost.StartAsync(CancellationToken.None).Wait();

                Console.WriteLine("Press ENTER/RETURN to close the test application.");
                Console.ReadLine();

                // Stop the FTP server
                ftpServerHost.StopAsync(CancellationToken.None).Wait();
            }
        }

        private static FileSystemInfo GetFileSystemObject(string basePath, string name)
        {
            string fullPath = Path.Combine(basePath, name);

            if (File.Exists(fullPath))
                return new FileInfo(fullPath);
            else if (Directory.Exists(fullPath))
                return new DirectoryInfo(fullPath);
            
            throw new FileNotFoundException("Could not find specified file or directory.");
        }
    }
}
