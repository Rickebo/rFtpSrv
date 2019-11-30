using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FubarDev.FtpServer.AccountManagement;
using Serilog;

namespace rFtpSrvFileSystem
{
    public class MembershipProvider : IMembershipProvider
    {
        public static Func<string, string, bool> MemberValidator = null;

        public Task<MemberValidationResult> ValidateUserAsync(string username, string password)
        {
            try
            {
                if (MemberValidator == null || !MemberValidator(username, password))
                    return Task.FromResult(new MemberValidationResult(MemberValidationStatus.InvalidLogin));
            }
            catch (Exception ex)
            {
                Log.Logger?.Error("An exception has occurred while validating user.", ex);
                return Task.FromResult(new MemberValidationResult(MemberValidationStatus.InvalidLogin));
            }

            return Task.FromResult(new MemberValidationResult(MemberValidationStatus.AuthenticatedUser, FtpUser.CreatePrincipal(username)));
        }
    }
}
