using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using FubarDev.FtpServer.AccountManagement;

namespace rFtpSrvFileSystem
{
    public class FtpUser : ClaimsPrincipal
    {
        public static ClaimsPrincipal CreatePrincipal(string username)
        {
            return new ClaimsPrincipal(new FtpUserIdentity(username));
        }
    }

    public class FtpUserIdentity : IIdentity
    {
        public string AuthenticationType => "authenticated";

        public bool IsAuthenticated => true;

        public string Name { get; }

        public FtpUserIdentity(string username)
        {
            Name = username;
        }
    }
}
