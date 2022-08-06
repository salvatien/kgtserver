using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dogs.Data.DataTransferObjects
{
    public class EventModel
    {
        public int EventId { get; set; }
        [Required]
        [Display(Name = "Nazwa")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Data")]
        public DateTime Date { get; set; }
        [Display(Name = "Miasto")]
        [Required]
        public string City { get; set; }
        [Display(Name = "Lokalizacja")]
        public string StreetOrLocation { get; set; }
        [Display(Name = "Opis")]
        public string Description { get; set; }
        [Display(Name = "Notatki")]
        public string Notes { get; set; }
        [Display(Name = "Trening komercyjny")]
        public bool IsCommercialTraining { get; set; }
        public List<int> GuideIds { get; set; }
        public List<int> DogIds { get; set; }
    }
}
