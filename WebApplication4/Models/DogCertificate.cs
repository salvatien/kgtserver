using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogsServer.Models
{
    public class DogCertificate
    {
        public int DogId { get; set; }
        public Dog Dog { get; set; }

        public int CertificateId { get; set; }
        public Certificate Certificate { get; set; }

        public DateTime AcquiredOn { get; set; }
    }
}
