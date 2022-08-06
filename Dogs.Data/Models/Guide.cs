using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dogs.Data.Models
{
    [Table("Guides")]
    public class Guide
    {
        [DatabaseGenerated(DatabaseGeneratedOption
           .Identity)]
        [Required]
        public int GuideId { get; set; }
        public string IdentityId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Notes { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsMember { get; set; }
        public virtual List<Dog>? Dogs { get; set; }
        public virtual IList<GuideEvent>? GuideEvents { get; set; }
    }
}