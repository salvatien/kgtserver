using System;
using System.Collections.Generic;
using System.Text;

namespace Dogs.ViewModels.Data.Models
{
    public class DogTraining
    {
        public int TrainingId { get; set; }
        public int DogId { get; set;}
        public string Person { get; set; }
        public string PersonTrack { get; set; }
        public string DogTrack { get; set; }
        //comments
    }
}
