using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dogs.Data.Models
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
        public int ValidThroughMonths { get; set; }
        public virtual List<DogCertificate> DogCertificates { get; set; }
    }
}
