using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dogs.Data.Models
{
    [Table("Trainings")]
    public class Training
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int TrainingId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string GeneralLocation { get; set; }
        public string? LocationDetails { get; set; }
        public string? Notes { get; set; }
        public string? Weather { get; set; }
        public virtual IList<DogTraining>? DogTrainings { get; set; }
        public virtual IList<TrainingComment>? Comments { get; set; }
    }
}