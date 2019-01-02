using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dogs.ViewModels.Data.Models
{
    public class EventModel
    {
        public int EventId { get; set; }
        [Display(Name = "Nazwa")]
        public string Title { get; set; }
        [Display(Name = "Data")]
        public DateTime Date { get; set; }
        [Display(Name = "Miasto")]
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
