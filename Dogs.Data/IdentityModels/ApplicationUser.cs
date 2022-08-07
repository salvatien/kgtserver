using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Dogs.Data.IdentityModels
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(200)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(250)]
        public string LastName { get; set; }

        public int KgtId { get; set; }

    }
}