﻿using Dogs.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dogs.Data.Models
{
    [Table("Dogs")]
    public class Dog
    {
        [DatabaseGenerated(DatabaseGeneratedOption
           .Identity)]
        [Required]
        public int DogId { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Breed { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public DogLevel Level { get; set; }
        public DogWorkmode? Workmodes { get; set; } // should it nullable?
        public string? PhotoBlobUrl { get; set; }
        public string? Notes { get; set; }
        [Required]
        public virtual Guide Guide { get; set; }
        public virtual IList<DogTraining>? DogTrainings { get; set; }
        public virtual IList<DogEvent>? DogEvents { get; set; }
        public virtual List<DogCertificate>? DogCertificates { get; set; }

    }
}
