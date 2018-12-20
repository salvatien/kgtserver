using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DogsServer.Models
{
    [Table("Actions")]
    public class Action
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ActionId")]
        [Required]
        public int ActionId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string StreetOrLocation { get; set; }
        [Required]
        public string LostPersonFirstName { get; set; }
        [Required]
        public string LostPersonLastName { get; set; }
        [Required]
        public string Coordinator { get; set; }
        [Required]
        public bool WasSuccess { get; set; }
        public string Notes { get; set; }
        public virtual IList<GuideAction> GuideActions { get; set; }
        public virtual IList<DogAction> DogActions { get; set; }
    }
}