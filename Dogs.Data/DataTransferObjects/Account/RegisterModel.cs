using System;
using System.ComponentModel.DataAnnotations;

namespace Dogs.Data.DataTransferObjects.Account
{
    public class RegisterModel : LoginModel
    {
        [Display(Name = "Imię")]
        public string FirstName { get; set; }
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }
        [Display(Name = "Adres Email")]
        public string Email { get; set; }
        [Display(Name = "Potwierdź hasło")]
        public string PasswordConfirmation { get; set; }
    }
}