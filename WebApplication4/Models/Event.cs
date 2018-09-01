using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DogsServer.Models
{
    [Table("Events")]
    public class Event
    {
        [DatabaseGenerated(DatabaseGeneratedOption
           .Identity)]
        [Required]
        public int EventID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string StreetOrLocation { get; set; }
        [Required]
        public string Description { get; set; }
        public string Notes { get; set; }
        public virtual IList<GuideEvent> GuideEvents { get; set; }
        public virtual List<Dog> Dogs { get; set; }
    }
}
