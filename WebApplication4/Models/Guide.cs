using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DogsServer.Models
{
    [Table("Guides")]
    public class Guide
    {
        [DatabaseGenerated(DatabaseGeneratedOption
           .Identity)]
        [Required]
        public int GuideID { get; set; }
        public string IdentityId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Notes { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsMember { get; set; }
        public virtual List<Dog> Dogs { get; set; }
        public virtual IList<GuideAction> GuideActions { get; set; }
        public virtual IList<GuideEvent> GuideEvents { get; set; }
    }
}
