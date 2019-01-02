using Dogs.ViewModels.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dogs.ViewModels.Data.Models
{
    public class DogModel
    {
        public int DogId { get; set; }
        [Display(Name = "Imię")]
        public string Name { get; set; }
        [Display(Name = "Rasa")]
        public string Breed { get; set; }
        [Display(Name = "Data urodzenia")]
        public DateTime DateOfBirth { get; set; }
        [Display(Name = "Poziom")]
        public DogLevel Level { get; set; }
        [Display(Name = "Tryb pracy")]
        public DogWorkmode? Workmodes { get; set; }
        [Display(Name = "Notatki")]
        public string Notes { get; set; }
        public string PhotoBlobUrl { get; set; }
        [Display(Name = "Przewodnik")]
        public IdNameModel GuideIdAndName { get; set; }
        public List<int> TrainingIds { get; set; }
        public List<int> EventIds { get; set; }
        public List<int> CertificateIds { get; set; }

    }
}