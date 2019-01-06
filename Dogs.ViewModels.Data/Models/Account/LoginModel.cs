using System;
using System.ComponentModel.DataAnnotations;

namespace Dogs.ViewModels.Data.Models.Account
{
    public class LoginModel
    {
        [Display(Name = "Login")]
        public String Username { get; set; }
        [Display(Name = "Hasło")]
        public String Password { get; set; }
    }
}
