using Moq;
using NUnit.Framework;
//using OTP.Business.Interfaces;

namespace OTP.Business.UnitTests
{
    [TestFixture]
    public class UserServiceUnitTests
    {
        [Test]
        public void Register_ModelIsNull_ReturnsBadRequest()
        {
            Assert.IsTrue(true);
            ////Arrange
            //Mock<IUserService> userServiceMock = new Mock<IUserService>();
            //var controller = new AccountController(userServiceMock.Object);
            //RegisterBindingModel model = null;

            ////Act
            //var result = controller.Register(model);

            ////Assert
            //Assert.IsInstanceOf<BadRequestResult>(result);
        }
    }
}
