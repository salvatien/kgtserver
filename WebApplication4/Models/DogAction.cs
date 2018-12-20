using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DogsServer.Models
{
    public class DogAction
    {
        [ForeignKey("Dog")]
        public int DogId { get; set; }
        public Dog Dog { get; set; }
        [ForeignKey("Action")]
        public int ActionId { get; set; }
        public Action Action { get; set; }
    }
}