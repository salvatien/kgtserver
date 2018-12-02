using System;
using System.ComponentModel.DataAnnotations;

namespace kgtwebClient.Models.Account
{
    public class LoginModel
    {
        [Required]
        public String Username { get; set; }

        [Required]
        public String Password { get; set; }
    }
}
