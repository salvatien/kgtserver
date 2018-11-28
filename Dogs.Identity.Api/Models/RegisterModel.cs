using System;
using System.ComponentModel.DataAnnotations;

namespace Dogs.Identity.Api.Models
{
    public class RegisterModel : LoginModel
    {
        [Required]
        [StringLength(200)]
        public String FirstName { get; set; }

        [Required]
        [StringLength(250)]
        public String LastName { get; set; }

        [Required]
        [EmailAddress]
        public String Email { get; set; }

        [Required]
        [Compare("Password")]
        //[RegularExpression("^.{4,20}$", ErrorMessage = "Hasło musi mieć minimum 4 i maksimum 20 znaków")]
        public String PasswordConfirmation { get; set; }
    }
}