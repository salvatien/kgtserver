using Dogs.ViewModels.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogs.ViewModels.Data.Models
{
    public class DogModel
    {
        public int DogId { get; set; }
        public string Name { get; set; }
        public string Breed { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DogLevel Level { get; set; }
        public DogWorkmode? Workmodes { get; set; }
        public string Notes { get; set; }
        public IdNameModel GuideIdAndName { get; set; }
    }
}
