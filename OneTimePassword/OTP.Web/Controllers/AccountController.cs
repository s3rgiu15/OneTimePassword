using OTP.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OTP.Models;

namespace OneTimePassword.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        public IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public ActionResult LoginTwoFactor([FromBody] LoginTwoFactorBindingModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            var userIdentityDetails = _userService.ValidateUserToken(model.UserId, model.Token, model.CurrentDateTime);
            return Ok(userIdentityDetails);
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public ActionResult Login([FromBody] LoginBindingModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            var userIdentityDetails = _userService.ValidateUserPassword(model.Email, model.Password);
            return Ok(userIdentityDetails);
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public ActionResult RegenerateToken([FromBody] RegenerateTokenBindingModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            var userIdentityDetails = _userService.RegenerateToken(model.UserId);
            return Ok(userIdentityDetails);
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public ActionResult Register([FromBody] RegisterBindingModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            var creationResult = _userService.CreateUser(model.Email, model.Password);
            return Ok(creationResult);
        }

    }
}
