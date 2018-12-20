using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogs.ViewModels.Data.Models
{
    public class GuideModel
    {
        public int GuideId { get; set; }
        public string IdentityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Notes { get; set; }
        public List<IdNameModel> Dogs { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsMember { get; set; }

    }
}
