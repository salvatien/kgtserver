﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dogs.Data.DataTransferObjects
{
    public class CertificateModel
    {
        public int CertificateId { get; set; }
        [Display(Name = "Nazwa")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "Poziom")]
        public string Level { get; set; }
        [Display(Name = "Opis")]
        public string Description { get; set; }
        [Display(Name = "Ważny przez (w miesiącach)")]
        public int ValidThroughMonths { get; set; }
        public virtual List<int> DogIds { get; set; }
    }
}
