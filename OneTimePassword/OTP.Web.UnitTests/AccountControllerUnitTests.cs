using OTP.Business.Interfaces;
using OneTimePassword.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OTP.Models;
using NUnit.Framework;
using System;

namespace OTP.Web.UnitTests
{
    [TestFixture]
    public class AccountControllerUnitTests
    {
        Mock<IUserService> userServiceMock;
        AccountController controller;

        [SetUp]
        public void Init()
        {
            userServiceMock = new Mock<IUserService>();
            controller = new AccountController(userServiceMock.Object);
        }

        [Test]
        public void Register_ModelIsNull_ReturnsBadRequest()
        {
            //Arrange
            RegisterBindingModel model = null;

            //Act
            var result = controller.Register(model);

            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public void Register_ValidModel_ReturnsOk()
        {
            //Arrange
            RegisterBindingModel model = new RegisterBindingModel
            {
                Email = "test@test.com",
                Password = "test"
            };
            userServiceMock.Setup(t => t.CreateUser(model.Email, model.Password)).Returns(new UserIdentityRegisterDto());

            //Act
            var result = controller.Register(model);

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void Login_ModelIsNull_ReturnsBadRequest()
        {
            //Arrange
            LoginBindingModel model = null;

            //Act
            var result = controller.Login(model);

            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public void Login_ValidModel_ReturnsOk()
        {
            //Arrange
            LoginBindingModel model = new LoginBindingModel
            {
                Email = "test@test.com",
                Password = "test"
            };
            userServiceMock.Setup(t => t.ValidateUserPassword(model.Email, model.Password)).Returns(new UserIdentityTwoFactorDto());

            //Act
            var result = controller.Login(model);

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void LoginTwoFactor_ModelIsNull_ReturnsBadRequest()
        {
            //Arrange
            LoginTwoFactorBindingModel model = null;

            //Act
            var result = controller.LoginTwoFactor(model);

            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public void LoginTwoFactor_ValidModel_ReturnsOk()
        {
            //Arrange
            LoginTwoFactorBindingModel model = new LoginTwoFactorBindingModel
            {
                UserId = "test",
                Token = "test",
                CurrentDateTime = DateTime.Now
            };
            userServiceMock.Setup(t => t.ValidateUserToken(model.UserId, model.Token, model.CurrentDateTime)).Returns(new UserIdentityTwoFactorDto());

            //Act
            var result = controller.LoginTwoFactor(model);

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void RegenerateToken_ModelIsNull_ReturnsBadRequest()
        {
            //Arrange
            RegenerateTokenBindingModel model = null;

            //Act
            var result = controller.RegenerateToken(model);

            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public void RegenerateToken_ValidModel_ReturnsOk()
        {
            //Arrange
            RegenerateTokenBindingModel model = new RegenerateTokenBindingModel
            {
                UserId = "test"
            };
            userServiceMock.Setup(t => t.RegenerateToken(model.UserId)).Returns(new UserIdentityTwoFactorDto());

            //Act
            var result = controller.RegenerateToken(model);

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }
    }

}
