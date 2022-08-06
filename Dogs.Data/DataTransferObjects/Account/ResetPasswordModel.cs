using System.ComponentModel.DataAnnotations;

namespace Dogs.Data.DataTransferObjects.Account
{
    public class ResetPasswordModel
    {
        [Required]
        [Display(Name = "Login lub Email")]
        public string UserNameOrEmail { get; set; }
        [Required]
        [Display(Name = "Nowe hasło")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Powtórz hasło")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
