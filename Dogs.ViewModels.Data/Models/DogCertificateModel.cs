using System;
using System.Collections.Generic;
using System.Text;

namespace Dogs.ViewModels.Data.Models
{
    public class DogCertificateModel
    {
        public int CertificateId { get; set; }
        public int DogId { get; set; }
        public DateTime AcquiredOn { get; set; }

    }
}
