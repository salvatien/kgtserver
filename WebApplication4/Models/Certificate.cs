using System;
using System.Collections.Generic;
using System.ComponentModel;
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


        //hack to implement timespan longer than 24 hours as ef core implements timespan as time in db, and time must be <24 hours
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Property '" + nameof(ValidThrough) + "' should be used instead.")]
        public long ValidThroughTicks { get; set; }

        [NotMapped]
        public TimeSpan ValidThrough
        {
#pragma warning disable 618
            get { return new TimeSpan(ValidThroughTicks); }
            set { ValidThroughTicks = value.Ticks; }
#pragma warning restore 618
        }


        public virtual List<DogCertificate> DogCertificates { get; set; }
    }
}
