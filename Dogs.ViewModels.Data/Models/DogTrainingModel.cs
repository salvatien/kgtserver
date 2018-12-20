using System;
using System.Collections.Generic;
using System.Text;

namespace Dogs.ViewModels.Data.Models
{
    public class DogTrainingModel
    {
        public List<Trkpt> PersonTrackpoints { get; set; }
        public List<Trkpt> DogTrackpoints { get; set; }
    }
}
