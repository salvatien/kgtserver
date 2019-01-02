﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dogs.ViewModels.Data.Models
{
    public class DogTrainingModel
    {
        [Display(Name = "Trening")]
        public int TrainingId { get; set; }
        public TrainingModel Training { get; set; }
        [Display(Name = "Pies")]
        public int DogId { get; set; }
        public DogModel Dog { get; set; }
        [Display(Name = "Pozorant")]
        public string LostPerson { get; set; }
        public string DogTrackBlobUrl { get; set; }
        public string LostPersonTrackBlobUrl { get; set; }
        [Display(Name = "Notatki")]
        public string Notes { get; set; }
        [Display(Name = "Komentarze")]
        public List<CommentModel> Comments { get; set; }
        [Display(Name = "Pogoda")]
        public string Weather { get; set; }
    }
}
