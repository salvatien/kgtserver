using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DogsServer.Models
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
