using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dogs.Data.Models
{
    [Table("DogCertificates")]
    public class DogCertificate
    {
        public int DogId { get; set; }
        public Dog Dog { get; set; }

        public int CertificateId { get; set; }
        public Certificate Certificate { get; set; }

        public DateTime AcquiredOn { get; set; }
    }
}
