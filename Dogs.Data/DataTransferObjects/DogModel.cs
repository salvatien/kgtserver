using Dogs.Data.DataTransferObjects.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dogs.Data.DataTransferObjects
{
    public class DogModel
    {
        public int DogId { get; set; }
        [Required]
        [Display(Name = "Imię")]
        public string Name { get; set; }
        [Display(Name = "Rasa")]
        public string Breed { get; set; }
        [Display(Name = "Data urodzenia")]
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Display(Name = "Poziom")]
        [Required]
        public DogLevel Level { get; set; }
        [Display(Name = "Tryb pracy")]
        public DogWorkmode? Workmodes { get; set; }
        [Display(Name = "Notatki")]
        public string Notes { get; set; }
        public string PhotoBlobUrl { get; set; }
        [Display(Name = "Przewodnik")]
        [Required]
        public IdNameModel GuideIdAndName { get; set; }
        public List<int> TrainingIds { get; set; }
        public List<int> EventIds { get; set; }
        public List<int> CertificateIds { get; set; }

    }
}