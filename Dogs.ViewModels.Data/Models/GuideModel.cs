using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogs.ViewModels.Data.Models
{
    public class GuideModel
    {
        public int GuideID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Fitness { get; set; }
        public string Notes { get; set; }
        public virtual List<int> DogIds { get; set; }
    }
}
