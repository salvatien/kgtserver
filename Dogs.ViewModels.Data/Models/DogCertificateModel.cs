using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dogs.ViewModels.Data.Models
{
    public class DogCertificateModel
    {
        [Display(Name = "Certyfikat")]
        public int CertificateId { get; set; }
        public CertificateModel Certificate { get; set; }
        [Display(Name = "Pies")]
        public int DogId { get; set; }
        public DogModel Dog { get; set; }
        [Display(Name = "Data uzyskania")]
        public DateTime AcquiredOn { get; set; }

    }
}
