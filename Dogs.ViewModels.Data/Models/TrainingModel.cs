﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dogs.ViewModels.Data.Models
{
    class TrainingModel
    {
        public int TrainingId { get; set; }
        public DateTime Date { get; set; }
        public List<DogModel> Dogs { get; set; }
        public string GeneralLocation { get; set; }
        public string LocationDetails { get; set; }
        public string Notes { get; set; }
    }
}
