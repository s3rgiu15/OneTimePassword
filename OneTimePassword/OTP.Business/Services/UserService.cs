using OTP.Business.Interfaces;
using Microsoft.AspNetCore.Identity;
using OTP.Models;
using System.Linq;
using System;
using Microsoft.Extensions.Options;

namespace OTP.Business.Services
{
    public class UserService : IUserService 
    {
        private readonly IUserManagerWrapper _userManagerWrapper;
        private readonly ITokenService _tokenService;
        private readonly TwoFactorTokenValability _twoFactorTokenConfiguration;

        public UserService(ITokenService tokenService, IUserManagerWrapper userManagerWrapper, IOptions<TwoFactorTokenValability> twoFactorTokenValability)
        {
            _tokenService = tokenService;
            _userManagerWrapper = userManagerWrapper;
            _twoFactorTokenConfiguration = twoFactorTokenValability.Value;
        }

        public UserIdentityRegisterDto CreateUser(string email, string password)
        {
            var user = new IdentityUser(email) { Email = email, EmailConfirmed = true, TwoFactorEnabled = true, LockoutEnabled = false };
            IdentityResult result = _userManagerWrapper.CreateUser(user, password);

            _userManagerWrapper.SetTwoFactorEnabled(user);

            if (result.Succeeded)
            {
                return new UserIdentityRegisterDto
                {
                    Error = ""
                };
            } else
            {
                return new UserIdentityRegisterDto
                {
                    Error = result.Errors.FirstOrDefault().Description
                };
            }

        }

        public UserIdentityTwoFactorDto ValidateUserToken(string userId, string token, DateTime currentDateTime)
        {
            var user = _userManagerWrapper.FindById(userId);

            if (user == null)
            {
                return GenerateUserIdentityTwoFactorDetailsError("Invalid UserId");
            }

            var tokenObj = _tokenService.GetTokenByUserId(userId);

            if (tokenObj == null)
            {
                return GenerateUserIdentityTwoFactorDetailsError("Token expired! Please generate a new one");
            }

            var diff = currentDateTime.Ticks - tokenObj.CreationDate.Ticks;

            if (TimeSpan.FromTicks(diff).TotalSeconds > _twoFactorTokenConfiguration.Seconds)
            {
                _tokenService.DeleteToken(userId);
                return GenerateUserIdentityTwoFactorDetailsError("Token expired! Please generate a new one");
            }

            var result = _userManagerWrapper.VerifyTwoFactorToken(user, token);

            if (!result)
            {
                return GenerateUserIdentityTwoFactorDetailsError("Invalid Token");
            }

            return GenerateUserIdentityTwoFactorDetails(user);

        }

        public UserIdentityTwoFactorDto ValidateUserPassword(string username, string password)
        {
            IdentityUser user = _userManagerWrapper.FindByName(username);
            if (user == null || !_userManagerWrapper.CheckPassword(user, password))
            {
                return GenerateUserIdentityTwoFactorDetailsError("Invalid Username or Password");
            }

            var twoFactorEnabled = _userManagerWrapper.GetTwoFactorEnabled(user);
            if (twoFactorEnabled)
            {
                var token = _userManagerWrapper.GenerateTwoFactorToken(user);
                return GetUserIdentityTwoFactorDetails(user, token);
            }

            return GenerateUserIdentityTwoFactorDetailsError("Two Factor Authentication is not enabled for this user");
        }

        public UserIdentityTwoFactorDto RegenerateToken(string userId)
        {
            var user = _userManagerWrapper.FindById(userId);

            if (user == null)
            {
                return GenerateUserIdentityTwoFactorDetailsError("Invalid UserId");
            }

            _tokenService.DeleteToken(userId);

            var twoFactorEnabled = _userManagerWrapper.GetTwoFactorEnabled(user);
            if (twoFactorEnabled)
            {
                var token = _userManagerWrapper.GenerateTwoFactorToken(user);
                return GetUserIdentityTwoFactorDetails(user, token);
            }

            return GenerateUserIdentityTwoFactorDetailsError("Two Factor Authentication is not enabled for this user");
        }

        private UserIdentityTwoFactorDto GetUserIdentityTwoFactorDetails(IdentityUser user, string token = "")
        {
            return GenerateUserIdentityTwoFactorDetails(user, token);
        }

        private UserIdentityTwoFactorDto GenerateUserIdentityTwoFactorDetails(IdentityUser user, string token = "")
        {
            return new UserIdentityTwoFactorDto
            {
                UserId = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Token = token.ToString(),
                Seconds = _twoFactorTokenConfiguration.Seconds,
                Error = ""
            };
        }

        private UserIdentityTwoFactorDto GenerateUserIdentityTwoFactorDetailsError(string error)
        {
            return new UserIdentityTwoFactorDto
            {
                UserId = "",
                Email = "",
                UserName = "",
                Token = "",
                Seconds = 0,
                Error = error
            };
        }
    }
}
