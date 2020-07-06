﻿using System;
using System.ComponentModel.DataAnnotations;

namespace OTP.DataAccess.Entities
{
    public class TokenExpiration
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
