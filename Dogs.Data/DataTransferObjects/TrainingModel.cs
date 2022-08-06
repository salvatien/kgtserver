using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dogs.Data.DataTransferObjects
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
        //previously [Display(Name = "Notatki")]
        [Display(Name = "Uwagi")]
        public string Notes { get; set; }
        [Display(Name = "Pogoda")]
        public string Weather { get; set; }
        [Display(Name = "Komentarze")]
        public List<CommentModel> Comments { get; set; }
    }
}
