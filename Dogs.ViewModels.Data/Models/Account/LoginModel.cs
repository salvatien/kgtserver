using System;
using System.ComponentModel.DataAnnotations;

namespace Dogs.ViewModels.Data.Models.Account
{
    public class LoginModel
    {
        [Display(Name = "Login")]
        public String Username { get; set; }
        [Display(Name = "Hasło - minimum 6 znaków, w tym co najmniej 1 cyfra")]
        public String Password { get; set; }
    }
}
