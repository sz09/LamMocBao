﻿using System;
using System.ComponentModel.DataAnnotations;

namespace LamMocBaoWeb.Models
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; } = true;

        public string HashValue { get; set; }
    }
}