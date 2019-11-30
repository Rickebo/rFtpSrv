using FubarDev.FtpServer;
using Microsoft.Extensions.DependencyInjection;
using rFtpSrvCore.FileSystem;
using rFtpSrvFileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using rLibCore2.Text;
using Serilog;
using Sodium;

namespace rFtpSrvCore
{
    class Program
    {
        private static string _settingsFile = "settings.json";
        private static string _usersFile = "users.json";

        private static IFtpServerHost _server;

        private static Settings _settings;
        private static UserSettings _userSettings;

        private static Dictionary<string, string> _validUsers = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                         .MinimumLevel.Debug()
                         .WriteTo.Console()
                         .WriteTo.RollingFile("logs\\log-{Date}.log")
                         .CreateLogger();

            _settingsFile = args != null && args.Length > 0
                ? args[0]
                : "settings.json";

            AddFiles();

            Log.Logger.Information("Reading settings...");
            _settings = ReadSettings(_settingsFile);

            // Setup dependency injection
            Log.Logger.Information("Setting up dependency injection...");
            var services = new ServiceCollection();

            // use %TEMP%/TestFtpServer as root folder
            Log.Logger.Information("Configuring file system options...");
            services.Configure<FileSystemOptions>(opt => opt
                .RootPath = Path.Combine(Environment.CurrentDirectory, _settings.FtpDirectory));

            Log.Logger.Information("Configuring file system options blocks...");
            ApplyBlocks(services);

            // Add FTP server services
            // DotNetFileSystemProvider = Use the .NET file system functionality
            // AnonymousMembershipProvider = allow only anonymous logins
            Log.Logger.Information("Adding ftp server service...");
            services.AddFtpServer(builder => builder
                .UseCustomFileSystem()
                .EnableCustomAuthentication());

            services.AddLogging(builder => builder.AddSerilog());

            // Configure the FTP server
            Log.Logger.Information("Configuring FTP server listen address...");
            services.Configure<FtpServerOptions>(opt => opt.ServerAddress = _settings.ListenAddress);

            Log.Logger.Information("Configuring FTP server listen port...");
            services.Configure<FtpServerOptions>(opt => opt.Port = _settings.ListenPort);

            // Build the service provider
            Log.Logger.Information("Building service provider...");
            using (var serviceProvider = services.BuildServiceProvider())
            {
                // Initialize the FTP server
                Log.Logger.Information("Initializing FTP server...");
                _server = serviceProvider.GetRequiredService<IFtpServerHost>();

                // Start the FTP server
                _server.StartAsync(CancellationToken.None).Wait();
                UpdateUserSettings();
                MembershipProvider.MemberValidator = ValidateUser;

                Console.WriteLine("See a list of supported commands by using typing \"help\"!");
                while (!InterpretCommand(Console.ReadLine()))
                {

                }
            }
        }

        private static void AddFiles()
        {
            if (System.IO.File.Exists(_settingsFile))
                return;

            File.WriteAllText(_settingsFile, Properties.Resources.DefaultSettings);

            if (File.Exists(_usersFile))
                return;

            File.WriteAllText(_usersFile, Properties.Resources.DefaultUsers);
        }

        private static void ApplyBlocks(ServiceCollection services)
        {
            services.Configure<FileSystemOptions>(opt => _settings.FileSettings.ForEach(file =>
            {
                if (file.AllowRegex)
                    opt.Blocks.AddBlock(new Regex(file.Path), file.GetOperation());
                else
                    opt.Blocks.AddBlock(GetFileSystemObject(_settings.FtpDirectory, file.Path), file.GetOperation());
            }));
        }

        private static bool InterpretCommand(string command)
        {
            Log.Logger.Information("Interpreting command: {@Command}", command);

            switch (command.ToLowerInvariant())
            {
                case "quit":
                case "exit":
                case "end":
                case "stop":
                    // Stop the FTP server
                    Log.Logger.Information("Stopping FTP server...");
                    _server.StopAsync(CancellationToken.None).Wait();
                    Log.Logger.Information("Exiting...");
                    return true;

                case "reload":
                    Log.Logger.Information("Updating user settings...");
                    UpdateUserSettings();
                    break;

                case "hash":
                    Console.WriteLine(Hash(Console.ReadLine()));
                    break;

                case "adduser":
                case "newuser":
                case "makeuser":
                case "createuser":
                    CreateUser();
                    break;

                case "help":
                    PrintHelp();
                    break;

                default:
                    Console.WriteLine("Could not understand that command, are you sure its spelled correctly?");
                    break;
            }

            return false;
        }


        private static void PrintHelp()
        {
            var lines = new []
            {
                "Supported commands are:",
                "- \"adduser\" - which prints the JSON required to add a specific user",
                "- \"hash\" - which hashes a specified password",
                "- \"reload\" - reloads user configuration file",
                "- \"stop\" - stops the FTP server"
            };

            foreach (var line in lines)
                Console.WriteLine(line);
        }

        private static void CreateUser()
        {
            Console.WriteLine("Username:");
            var username = Console.ReadLine();
            Console.WriteLine("Password:");
            var password = Console.ReadLine();

            if (!AreCredentialsValid(username, password))
            {
                Console.WriteLine("Invalid username or password. The following requirements apply:");
                Console.WriteLine("Usernames must be at least 3 characters long, and at most 32 characters long.");
                Console.WriteLine("Passwords must be at least 8 characters long, and at most 64 characters long.");
                return;
            }

            var hash = Hash(password);

            Console.WriteLine("Enter the following JSON in your users configuration file, and then enter the");
            Console.WriteLine("command \"reload\" in this console to reload the user configuration file.");
            Console.WriteLine();
            Console.WriteLine("{");
            Console.WriteLine("    \"Username\": \"" + username.ToLowerInvariant() + "\",");
            Console.WriteLine("    \"PasswordHash\": \"" + hash + "\"");
            Console.WriteLine("}");
        }

        private static void UpdateUserSettings()
        {
            _userSettings = ReadUsers(_settings.UsersFile);
            var newUsers = new Dictionary<string, string>();

            _userSettings.Users.ForEach(user => newUsers.Add(user.UserName.ToLowerInvariant(), user.PasswordHash));
            _validUsers = newUsers;
        }

        private static bool AreCredentialsValid(string username, string password)
        {
            return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) 
                && username.Length >= 3 && password.Length >= 8
                && username.Length <= 32 && password.Length <= 64;
        }

        private static bool ValidateUser(string username, string password)
        {
            if (!AreCredentialsValid(username, password))
            {
                FailValidation(username);
                return false;
            }

            if (_validUsers.TryGetValue(username.ToLowerInvariant(), out var userHash) &&
                Hash(password).Equals(userHash, StringComparison.OrdinalIgnoreCase))
            {
                Log.Logger.Information("User {@Username} has been validated successfully.", username);
                return true;
            }

            FailValidation(username);
            return false;
        }

        private static void FailValidation(string username)
        {
            Log.Logger.Information("The user {@Username} is failed to validate.", username);
        }

        private static string Hash(string text)
        {
            var hash = PasswordHash.ScryptHashBinary(text, _settings.PasswordSalt, PasswordHash.Strength.Medium, 32);

            return hash.ToHexString();
        }

        private static FileSystemInfo GetFileSystemObject(string basePath, string name)
        {
            var fullPath = Path.Combine(basePath, name);

            if (File.Exists(fullPath))
                return new FileInfo(fullPath);
            
            if (Directory.Exists(fullPath))
                return new DirectoryInfo(fullPath);
            
            throw new FileNotFoundException("Could not find specified file or directory.");
        }

        private static UserSettings ReadUsers(string filename) => ReadJson<UserSettings>(filename);
        private static Settings ReadSettings(string filename) => ReadJson<Settings>(filename);

        private static T ReadJson<T>(string filename)
        {
            var json = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
