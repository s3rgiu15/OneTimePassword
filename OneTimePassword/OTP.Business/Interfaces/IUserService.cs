using OTP.Models;
using System;

namespace OTP.Business.Interfaces
{
    public interface IUserService
    {
        UserIdentityTwoFactorDto ValidateUserPassword(string username, string password);
        UserIdentityRegisterDto CreateUser(string email, string password);
        UserIdentityTwoFactorDto ValidateUserToken(string userId, string token, DateTime currentDateT);
        UserIdentityTwoFactorDto RegenerateToken(string userId);
    }
}
