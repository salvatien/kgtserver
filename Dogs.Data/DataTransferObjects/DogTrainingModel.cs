using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dogs.Data.DataTransferObjects
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
        // previously [Display(Name = "Notatki")]
        [Display(Name = "Uwagi przewodnika")]
        public string Notes { get; set; }
        [Display(Name = "Rodzaj podłoża")]
        public string GroundType { get; set; }
        [Display(Name = "Komentarze")]
        public List<CommentModel> Comments { get; set; }
        [Display(Name = "Pogoda")]
        public string Weather { get; set; }
        [Display(Name = "Długość śladu")]
        public double LostPersonTrackLength { get; set; }
        [Display(Name = "Czas odłożenia")]
        public TimeSpan DelayTime { get; set; }
        [Display(Name = "Dodatkowy obraz")]
        public string AdditionalPictureBlobUrl { get; set; }
    }
}
