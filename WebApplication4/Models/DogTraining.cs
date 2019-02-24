using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public double LostPersonTrackLength { get; set; }
        public string GroundType { get; set; }
        //hack to implement timespan longer than 24 hours as ef core implements timespan as time in db, and time must be <24 hours
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Property '" + nameof(DelayTime) + "' should be used instead.")]
        public long DelayTimeTicks { get; set; }

        [NotMapped]
        public TimeSpan DelayTime
        {
#pragma warning disable 618
            get { return new TimeSpan(DelayTimeTicks); }
            set { DelayTimeTicks = value.Ticks; }
#pragma warning restore 618
        }


    }
}
