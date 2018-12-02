using System;

namespace Dogs.ViewModels.Data.Models.Account
{
    public class RegisterModel : LoginModel
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
        public String PasswordConfirmation { get; set; }
    }
}