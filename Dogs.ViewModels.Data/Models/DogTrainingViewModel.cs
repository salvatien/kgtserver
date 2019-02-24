using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dogs.ViewModels.Data.Models
{
    public class DogTrainingViewModel
    {
        public TrainingModel Training { get; set; }
        [Display(Name = "Trening")]
        public int TrainingId { get; set; }
        public DogModel Dog { get; set; }
        [Display(Name = "Pies")]
        public int DogId { get; set; }
        [Display(Name = "Pozorant")]
        public string LostPerson { get; set; }
        public string DogTrackFilename { get; set; }
        public string LostPersonTrackFilename { get; set; }
        public List<Trkpt> DogTrackPoints { get; set; }
        public List<Trkpt> LostPersonTrackPoints { get; set; }
        [Display(Name = "Komentarze")]
        public List<CommentModel> Comments { get; set; }
        [Display(Name = "Czas startu pozoranta")]
        public DateTime TimeOfLostPersonStart { get; set; }
        [Display(Name = "Czas startu psa")]
        public DateTime TimeOfDogStart { get; set; }
        [Display(Name = "Czas pracy psa")]
        public TimeSpan Duration { get; set; }
        [Display(Name = "Czas odłożenia śladu")]
        public TimeSpan DelayTime { get; set; }
        [Display(Name = "Długość śladu pozoranta (w metrach)")]
        public double LostPersonTrackLength { get; set; }
        [Display(Name = "Długość śladu psa (w metrach)")]
        public double DogTrackLength { get; set; }
        //previously [Display(Name = "Notatki")]
        [Display(Name = "Uwagi przewodnika")]
        public string Notes { get; set; }
        [Display(Name = "Rodzaj podłoża")]
        public string GroundType { get; set; }
        [Display(Name = "Cel")]
        public string Goal { get; set; }
        [Display(Name = "Pogoda")]
        public string Weather { get; set; } //overrides entire training's weather if not empty

    }
}
