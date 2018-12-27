using System;
using System.Collections.Generic;
using System.Text;

namespace Dogs.ViewModels.Data.Models
{
    public class CertificateModel
    {
        public int CertificateId { get; set; }
        public string Name { get; set; }
        public string Level { get; set; }
        public string Description { get; set; }
        public TimeSpan ValidThrough { get; set; }
        public virtual List<int> DogIds { get; set; }
    }
}
