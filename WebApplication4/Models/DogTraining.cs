using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DogsServer.Models
{
    [Table("DogTrainings")]
    public class DogTraining
    {
        [Required]
        public int TrainingId { get; set; }
        public virtual Training Training { get; set; }
        [Required]
        public int DogId { get; set; }
        public virtual Dog Dog { get; set; }
        public string LostPerson { get; set; }
        public string DogTrackBlobUrl { get; set; }
        public string LostPersonTrackBlobUrl { get; set; }
        public string Notes { get; set; }
        public virtual IList<DogTrainingComment> Comments { get; set; }
        public string Weather { get; set; }
    }
}
