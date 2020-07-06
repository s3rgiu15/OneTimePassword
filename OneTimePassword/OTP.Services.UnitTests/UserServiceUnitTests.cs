using OTP.Business.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OTP.Business.Interfaces;
using OTP.DataAccess.Entities;
using OTP.Models;
using System;

namespace OTP.Services.UnitTests
{
    [TestClass]
    public class UserServiceUnitTests
    {
        private Mock<IUserManagerWrapper> _userManagerWrapper;
        private Mock<ITokenService> _tokenService;
        private IOptions<TwoFactorTokenValability> _twoFactorTokenConfiguration;
        private UserService userService;

        [TestInitialize]
        public void Init()
        {
            _userManagerWrapper = new Mock<IUserManagerWrapper>();
            _tokenService = new Mock<ITokenService>();
            _twoFactorTokenConfiguration = Options.Create<TwoFactorTokenValability>(new TwoFactorTokenValability { Seconds = 30 });
            userService = new UserService(_tokenService.Object, _userManagerWrapper.Object, _twoFactorTokenConfiguration);
        }

        [TestMethod]
        public void CreateUser_ValidCredentials_ReturnsNoError()
        {
            //Arrange
            _userManagerWrapper.Setup(u => u.CreateUser(It.IsAny<IdentityUser>(), It.IsAny<string>())).Returns(IdentityResult.Success);

            //Act
            var result = userService.CreateUser("", "");

            //Assert
            Assert.IsTrue(result.Error == "");
        }

        [TestMethod]
        public void ValidateUserToken_InvalidUserId_ReturnsError()
        {
            //Arrange
            _userManagerWrapper.Setup(u => u.FindById(It.IsAny<string>())).Returns((IdentityUser)null);

            //Act
            var result = userService.ValidateUserToken("", "", It.IsAny<DateTime>());

            //Assert
            Assert.IsTrue(result.Error == "Invalid UserId");
        }

        [TestMethod]
        public void ValidateUserToken_TokenNotAvailableInTable_ReturnsError()
        {
            //Arrange
            _userManagerWrapper.Setup(u => u.FindById(It.IsAny<string>())).Returns(new IdentityUser());
            _tokenService.Setup(t => t.GetTokenByUserId(It.IsAny<string>())).Returns((TokenExpiration)null);

            //Act
            var result = userService.ValidateUserToken("", "", It.IsAny<DateTime>());

            //Assert
            Assert.IsTrue(result.Error == "Token expired! Please generate a new one");
        }

        [TestMethod]
        public void ValidateUserToken_TokenExpired_ReturnsError()
        {
            //Arrange
            _userManagerWrapper.Setup(u => u.FindById(It.IsAny<string>())).Returns(new IdentityUser());
            _tokenService.Setup(t => t.GetTokenByUserId(It.IsAny<string>())).Returns(new TokenExpiration { CreationDate = DateTime.Now });

            //Act
            var dateTime = DateTime.Now.AddSeconds(40);
            var result = userService.ValidateUserToken("", "", dateTime);

            //Assert
            Assert.IsTrue(result.Error == "Token expired! Please generate a new one");
        }

        [TestMethod]
        public void ValidateUserToken_WrongToken_ReturnsError()
        {
            //Arrange
            _userManagerWrapper.Setup(u => u.FindById(It.IsAny<string>())).Returns(new IdentityUser());
            _tokenService.Setup(t => t.GetTokenByUserId(It.IsAny<string>())).Returns(new TokenExpiration { CreationDate = DateTime.Now });
            _userManagerWrapper.Setup(u => u.VerifyTwoFactorToken(It.IsAny<IdentityUser>(), It.IsAny<string>())).Returns(false);

            //Act
            var dateTime = DateTime.Now.AddSeconds(10);
            var result = userService.ValidateUserToken("", "", dateTime);

            //Assert
            Assert.IsTrue(result.Error == "Invalid Token");
        }

        [TestMethod]
        public void ValidateUserToken_ValidToken_ReturnsNoError()
        {
            //Arrange
            _userManagerWrapper.Setup(u => u.FindById(It.IsAny<string>())).Returns(new IdentityUser());
            _tokenService.Setup(t => t.GetTokenByUserId(It.IsAny<string>())).Returns(new TokenExpiration { CreationDate = DateTime.Now });
            _userManagerWrapper.Setup(u => u.VerifyTwoFactorToken(It.IsAny<IdentityUser>(), It.IsAny<string>())).Returns(true);

            //Act
            var dateTime = DateTime.Now.AddSeconds(10);
            var result = userService.ValidateUserToken("", "", dateTime);

            //Assert
            Assert.IsTrue(result.Error == "");
        }

        [TestMethod]
        public void ValidateUserPassword_InvalidUserName_ReturnsError()
        {
            //Arrange
            _userManagerWrapper.Setup(u => u.FindByName(It.IsAny<string>())).Returns((IdentityUser)null);

            //Act
            var result = userService.ValidateUserPassword("", "");

            //Assert
            Assert.IsTrue(result.Error == "Invalid Username or Password");
        }

        [TestMethod]
        public void ValidateUserPassword_InvalidPassword_ReturnsError()
        {
            //Arrange
            _userManagerWrapper.Setup(u => u.FindByName(It.IsAny<string>())).Returns(new IdentityUser());
            _userManagerWrapper.Setup(u => u.CheckPassword(It.IsAny<IdentityUser>(), It.IsAny<string>())).Returns(false);

            //Act
            var result = userService.ValidateUserPassword("", "");

            //Assert
            Assert.IsTrue(result.Error == "Invalid Username or Password");
        }

        [TestMethod]
        public void ValidateUserPassword_TwoFactorDisabled_ReturnsError()
        {
            //Arrange
            _userManagerWrapper.Setup(u => u.FindByName(It.IsAny<string>())).Returns(new IdentityUser());
            _userManagerWrapper.Setup(u => u.CheckPassword(It.IsAny<IdentityUser>(), It.IsAny<string>())).Returns(true);
            _userManagerWrapper.Setup(u => u.GetTwoFactorEnabled(It.IsAny<IdentityUser>())).Returns(false);

            //Act
            var result = userService.ValidateUserPassword("", "");

            //Assert
            Assert.IsTrue(result.Error == "Two Factor Authentication is not enabled for this user");
        }

        [TestMethod]
        public void ValidateUserPassword_TwoFactorEnabled_ReturnsNoError()
        {
            //Arrange
            _userManagerWrapper.Setup(u => u.FindByName(It.IsAny<string>())).Returns(new IdentityUser());
            _userManagerWrapper.Setup(u => u.CheckPassword(It.IsAny<IdentityUser>(), It.IsAny<string>())).Returns(true);
            _userManagerWrapper.Setup(u => u.GetTwoFactorEnabled(It.IsAny<IdentityUser>())).Returns(true);
            _userManagerWrapper.Setup(u => u.GenerateTwoFactorToken(It.IsAny<IdentityUser>())).Returns("token");

            //Act
            var result = userService.ValidateUserPassword("", "");

            //Assert
            Assert.IsTrue(result.Error == "");
        }

        [TestMethod]
        public void RegenerateToken_InvalidUserId_ReturnsError()
        {
            //Arrange
            _userManagerWrapper.Setup(u => u.FindById(It.IsAny<string>())).Returns((IdentityUser)null);

            //Act
            var result = userService.RegenerateToken("");

            //Assert
            Assert.IsTrue(result.Error == "Invalid UserId");
        }

        [TestMethod]
        public void RegenerateToken_TwoFactorDisabled_ReturnsError()
        {
            //Arrange
            _userManagerWrapper.Setup(u => u.FindById(It.IsAny<string>())).Returns(new IdentityUser());
            _userManagerWrapper.Setup(u => u.GetTwoFactorEnabled(It.IsAny<IdentityUser>())).Returns(false);

            //Act
            var result = userService.RegenerateToken("");

            //Assert
            Assert.IsTrue(result.Error == "Two Factor Authentication is not enabled for this user");
        }

        [TestMethod]
        public void RegenerateToken_TwoFactorEnabled_ReturnsNoError()
        {
            //Arrange
            _userManagerWrapper.Setup(u => u.FindById(It.IsAny<string>())).Returns(new IdentityUser());
            _userManagerWrapper.Setup(u => u.GetTwoFactorEnabled(It.IsAny<IdentityUser>())).Returns(true);
            _userManagerWrapper.Setup(u => u.GenerateTwoFactorToken(It.IsAny<IdentityUser>())).Returns("token");

            //Act
            var result = userService.RegenerateToken("");

            //Assert
            Assert.IsTrue(result.Error == "");
        }
    }
}
