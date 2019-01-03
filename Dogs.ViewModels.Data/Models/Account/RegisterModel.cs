using System;
using System.ComponentModel.DataAnnotations;

namespace Dogs.ViewModels.Data.Models.Account
{
    public class RegisterModel : LoginModel
    {
        [Display(Name = "Imię")]
        public String FirstName { get; set; }
        [Display(Name = "Nazwisko")]
        public String LastName { get; set; }
        [Display(Name = "Adres Email")]
        public String Email { get; set; }
        [Display(Name = "Potwierdź hasło")]
        public String PasswordConfirmation { get; set; }
    }
}