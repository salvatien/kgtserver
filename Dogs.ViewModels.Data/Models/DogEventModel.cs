using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dogs.ViewModels.Data.Models
{
    public class DogEventModel
    {
        [Display(Name = "Pies")]
        public int DogId { get; set; }
        public DogModel Dog { get; set; }
        [Display(Name = "Wydarzenie")]
        public int EventId { get; set; }
        public EventModel Event { get; set; }
        [Display(Name = "Pozorant")]
        public string LostPerson { get; set; }
        public string DogTrackBlobUrl { get; set; }
        public string LostPersonTrackBlobUrl { get; set; }
        [Display(Name = "Notatki")]
        public string Notes { get; set; }
        [Display(Name = "Pogoda")]
        public string Weather { get; set; }
    }
}
