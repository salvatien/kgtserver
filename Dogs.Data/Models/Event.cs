using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dogs.Data.Models
{
    [Table("Events")]
    public class Event
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Column("EventId")]
        public int EventId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string? City { get; set; }
        public string? StreetOrLocation { get; set; }
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public bool IsCommercialTraining { get; set; }
        public virtual IList<GuideEvent>? GuideEvents { get; set; }
        public virtual IList<DogEvent>? DogEvents { get; set; }
    }
}