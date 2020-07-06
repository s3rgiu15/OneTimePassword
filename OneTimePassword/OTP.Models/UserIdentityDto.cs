using System;
using System.Collections.Generic;
using System.Text;

namespace OTP.Models
{
    public class UserIdentityDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Features { get; set; }
    }
}
