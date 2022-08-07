using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dogs.Data.DataTransferObjects
{
    public class GuideModel
    {
        public int GuideId { get; set; }
        public string IdentityId { get; set; }
        [Display(Name = "Imię")]
        [Required]
        public string FirstName { get; set; }
        [Display(Name = "Nazwisko")]
        [Required]
        public string LastName { get; set; }
        [Display(Name = "Email")]
        [Required]
        public string Email { get; set; }
        [Display(Name = "Miasto")]
        public string City { get; set; }
        [Display(Name = "Adres")]
        public string Address { get; set; }
        [Display(Name = "Numer telefonu")]
        public string Phone { get; set; }
        [Display(Name = "Notatki")]
        public string Notes { get; set; }
        [Display(Name = "Psy")]
        public List<IdNameModel> Dogs { get; set; }
        [Display(Name = "Administrator")]
        public bool IsAdmin { get; set; }
        [Display(Name = "Członek grupy")]
        public bool IsMember { get; set; }
        public List<int> EventIds { get; set; }
    }
}
