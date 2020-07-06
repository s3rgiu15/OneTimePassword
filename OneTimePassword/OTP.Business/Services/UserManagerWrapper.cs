using OTP.Business.Interfaces;
using Microsoft.AspNetCore.Identity;
using OTP.Models;
using System.Linq;
using System;

namespace Business.Services
{
    public class UserManagerWrapper : IUserManagerWrapper 
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserManagerWrapper(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IdentityResult CreateUser(IdentityUser user, string password)
        {
            return _userManager.CreateAsync(user, password).Result;
        }

        public void SetTwoFactorEnabled(IdentityUser user)
        {
            _userManager.SetTwoFactorEnabledAsync(user, true);
        }

        public IdentityUser FindById(string userId)
        {
            return _userManager.FindByIdAsync(userId).Result;
        }

        public bool VerifyTwoFactorToken(IdentityUser user, string token)
        {
            return _userManager.VerifyTwoFactorTokenAsync(user, "Custom", token).Result;
        }

        public IdentityUser FindByName(string username)
        {
            return _userManager.FindByNameAsync(username).Result;
        }

        public bool CheckPassword(IdentityUser user, string password) 
        {
            return _userManager.CheckPasswordAsync(user, password).Result;
        }

        public bool GetTwoFactorEnabled(IdentityUser user)
        {
            return _userManager.GetTwoFactorEnabledAsync(user).Result;
        }

        public string GenerateTwoFactorToken(IdentityUser user)
        {
            return _userManager.GenerateTwoFactorTokenAsync(user, "Custom").Result;
        }
    }
}
