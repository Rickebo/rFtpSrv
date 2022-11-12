# rFtpSrv

An extension of [FubarDev's portable cross-platform FTP server](https://github.com/FubarDevelopment/FtpServer) to add support for restricting access to certain files or directories.

The server is configurable by placing a `settings.json` file in its working directory. All available settings can be are
the properties of the [Settings class](https://github.com/Rickebo/rFtpSrv/blob/master/rFtpSrvCore/Settings.cs). 

To use the server, create and configure a settings.json file to your liking, and start the server. Create new users with
the adduser command, paste the generated user JSON to the users.json file and restart the server.

## Prerequisites

- [.NET Core 3.0](https://dotnet.microsoft.com/en-us/download/dotnet/3.0)