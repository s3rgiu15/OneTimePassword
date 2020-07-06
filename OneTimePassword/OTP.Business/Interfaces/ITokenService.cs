using System;
using OTP.DataAccess.Entities;

namespace OTP.Business.Interfaces
{
    public interface ITokenService
    {
        void AddToken(string userId, string token, DateTime CreationDate);
        TokenExpiration GetTokenByUserId(string userId);
        void DeleteToken(string userId);
    }
}
