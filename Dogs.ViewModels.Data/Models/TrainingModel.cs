using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dogs.ViewModels.Data.Models
{
    public class TrainingModel
    {
        public int TrainingId { get; set; }
        [Display(Name = "Data")]
        [Required]
        public DateTime Date { get; set; }
        [Display(Name = "Treningi psa")]
        public List<DogTrainingModel> DogTrainings { get; set; }
        [Display(Name = "Lokalizacja ogólna")]
        [Required]
        public string GeneralLocation { get; set; }
        [Display(Name = "Lokalizacja szczegółowa")]
        public string LocationDetails { get; set; }
        [Display(Name = "Notatki")]
        public string Notes { get; set; }
        [Display(Name = "Pogoda")]
        public string Weather { get; set; }
        [Display(Name = "Komentarze")]
        public List<CommentModel> Comments { get; set; }
    }
}
