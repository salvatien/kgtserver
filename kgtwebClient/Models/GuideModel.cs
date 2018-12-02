using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace kgtwebClient.Models
{
    [Table("Guides")]
    public class GuideModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption
           .Identity)]
        [Required]
        public int GuideID { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Phone { get; set; }
        public string Fitness { get; set; }
        public string Notes { get; set; }
        public virtual List<DogModel> Dogs { get; set; }
        public virtual IList<GuideActionModel> GuideActions { get; set; }
        public virtual IList<GuideEventModel> GuideEvents { get; set; }
    }
}
