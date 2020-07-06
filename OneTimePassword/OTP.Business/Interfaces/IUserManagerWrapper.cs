using Microsoft.AspNetCore.Identity;

namespace OTP.Business.Interfaces
{
    public interface IUserManagerWrapper
    {
        IdentityResult CreateUser(IdentityUser user, string password);
        void SetTwoFactorEnabled(IdentityUser user);

        IdentityUser FindById(string userId);
        bool VerifyTwoFactorToken(IdentityUser user, string token);
        IdentityUser FindByName(string username);
        bool CheckPassword(IdentityUser user, string password);
        bool GetTwoFactorEnabled(IdentityUser user);
        string GenerateTwoFactorToken(IdentityUser user);
    }
}
