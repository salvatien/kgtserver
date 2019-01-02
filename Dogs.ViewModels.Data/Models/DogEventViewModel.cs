using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dogs.ViewModels.Data.Models
{
    public class DogEventViewModel
    {
        public EventModel Event { get; set; }
        [Display(Name = "Wydarzenie")]
        public int EventId { get; set; }
        [Display(Name = "Pies")]
        public DogModel Dog { get; set; }
        public int DogId { get; set; }
        [Display(Name = "Pozorant")]
        public string LostPerson { get; set; }
        public string DogTrackFilename { get; set; }
        public string LostPersonTrackFilename { get; set; }
        public List<Trkpt> DogTrackPoints { get; set; }
        public List<Trkpt> LostPersonTrackPoints { get; set; }
        public List<CommentModel> Comments { get; set; }
        [Display(Name = "Czas startu pozoranta")]
        public DateTime TimeOfLostPersonStart { get; set; }
        [Display(Name = "Czas startu psa")]
        public DateTime TimeOfDogStart { get; set; }
        [Display(Name = "Czas pracy psa")]
        public TimeSpan Duration { get; set; }
        [Display(Name = "Długość śladu pozoranta")]
        public double LostPersonTrackLength { get; set; }
        [Display(Name = "Długość śladu psa")]
        public double DogTrackLength { get; set; }
        [Display(Name = "Notatki")]
        public string Notes { get; set; }
        [Display(Name = "Cel")]
        public string Goal { get; set; }
        [Display(Name = "Pogoda")]
        public string Weather { get; set; } //overrides entire training's weather if not empty
    }
}
