using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dogs.ViewModels.Data.Models
{
    public class IdNameModel
    {
        public int Id { get; set; }
        [Display(Name = "Przewodnik")]      //TODO check if this is used to display only Guide name
        public string Name { get; set; }
    }
}
