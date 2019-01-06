using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dogs.ViewModels.Data.Models
{
    public class GuideModel
    {
        public int GuideId { get; set; }
        public string IdentityId { get; set; }
        [Display(Name = "Imię")]
        public string FirstName { get; set; }
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }
        [Display(Name = "Email")]
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
