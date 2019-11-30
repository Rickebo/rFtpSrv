using System;
using System.Collections.Generic;
using System.Text;

namespace rFtpSrvCore
{
    public class UserSettings
    {
        public List<FtpUserSettings> Users { get; set; }
    }

    public class FtpUserSettings
    {
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
    }
}
