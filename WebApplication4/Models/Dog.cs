using DogsServer.Models.Enums;
using DogsServer.Models.Enums.Wrappers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DogsServer.Models
{
    [Table("Dogs")]
    public class Dog
    {
        [DatabaseGenerated(DatabaseGeneratedOption
           .Identity)]
        [Required]
        public int DogID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public DogLevel Level { get; set; }
        [Required]
        public List<DogWorkmodeWrapper> Workmode { get; set; }
        public string Notes { get; set; }
        [Required]
        public virtual Guide Guide { get; set; }


    }
}
