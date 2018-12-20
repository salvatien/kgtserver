using System;
using System.Collections.Generic;
using System.Text;

namespace Dogs.ViewModels.Data.Models
{
    public class DogTrainingModel
    {
        public int TrainingId { get; set; }
        public TrainingModel Training { get; set; }
        public int DogId { get; set; }
        public DogModel Dog { get; set; }
        public string LostPerson { get; set; }
        public string DogTrackBlobUrl { get; set; }
        public string LostPersonTrackBlobUrl { get; set; }
        public List<CommentModel> Comments { get; set; }
        public string Weather { get; set; }
    }
}
