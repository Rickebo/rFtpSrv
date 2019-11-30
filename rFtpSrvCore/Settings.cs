using System;
using System.Collections.Generic;
using System.Text;
using rFtpSrvFileSystem;

namespace rFtpSrvCore
{
    public class Settings
    {
        private string _passwordSalt = "ZArP7hHZAfkWtT9Ubh2jCDHpcDXeBzHx";

        public string FtpDirectory { get; set; }
        public string ListenAddress { get; set; }
        public int ListenPort { get; set; }
        public string UsersFile { get; set; }

        public string PasswordSalt
        {
            get => _passwordSalt;
            set
            {
                if (Encoding.ASCII.GetBytes(value).Length != 32)
                    throw new ArgumentException("Length of password salt must be 32 bytes (using ASCII encoding).");

                _passwordSalt = value;
            }
        } 

        public List<FileSettings> FileSettings { get; set; }
    }

    public class FileSettings
    {
        public string Path { get; set; }
        public bool AllowRegex { get; set; } = false;

        public bool BlockList { get; set; } = false;
        public bool BlockCreate { get; set; } = false;
        public bool BlockDelete { get; set; } = false;
        public bool BlockRead { get; set; } = false;
        public bool BlockWrite { get; set; } = false;
        public bool BlockMove { get; set; } = false;
        public bool BlockCopy { get; set; } = false;
        public bool BlockAccess { get; set; } = false;
        public bool BlockUnlink { get; set; } = false;
        public bool BlockLink { get; set; } = false;
        public bool BlockSetTime { get; set; } = false;
        public bool BlockAll { get; set; } = false;

        public BlockOperation GetOperation()
        {
            BlockOperation ret = default;

            if (BlockList)
                ret |= BlockOperation.List;

            if (BlockCreate)
                ret |= BlockOperation.Create;

            if (BlockDelete)
                ret |= BlockOperation.Delete;

            if (BlockRead)
                ret |= BlockOperation.Read;

            if (BlockWrite)
                ret |= BlockOperation.Write;

            if (BlockMove)
                ret |= BlockOperation.Move;

            if (BlockCopy)
                ret |= BlockOperation.Copy;

            if (BlockAccess)
                ret |= BlockOperation.Access;

            if (BlockUnlink)
                ret |= BlockOperation.Unlink;

            if (BlockLink)
                ret |= BlockOperation.Link;

            if (BlockSetTime)
                ret |= BlockOperation.SetTime;

            if (BlockAll)
                ret |= BlockOperation.All;

            return ret;
        }
    }
}
