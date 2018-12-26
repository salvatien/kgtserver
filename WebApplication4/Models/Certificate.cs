using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DogsServer.Models
{
    [Table("Certificates")]
    public class Certificate
    {
        [DatabaseGenerated(DatabaseGeneratedOption
            .Identity)]
        [Required]
        public int CertificateId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Level { get; set; }
        public string Description { get; set; }
        public TimeSpan ValidThrough { get; set; }
        public virtual List<DogCertificate> DogCertificates { get; set; }
    }
}
