namespace OTP.Models
{
    public class UserIdentityTwoFactorDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public int Seconds { get; set; }
        public string Error { get; set; }
    }
}
