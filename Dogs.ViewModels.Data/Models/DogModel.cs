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
        public DateTime DateOfBirth { get; set; }
        public DogLevel Level { get; set; }
        public DogWorkmode? Workmodes { get; set; }
        public string Notes { get; set; }
        public string PhotoBlobUrl { get; set; }
        public IdNameModel GuideIdAndName { get; set; }
        public List<int> TrainingIds { get; set; }
        public List<int> EventIds { get; set; }
        public List<int> CertificateIds { get; set; }

    }
}