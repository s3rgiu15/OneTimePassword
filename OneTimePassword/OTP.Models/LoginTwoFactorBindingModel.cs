using System;

namespace OTP.Models
{
    public class LoginTwoFactorBindingModel
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime CurrentDateTime { get; set; }
    }
}
