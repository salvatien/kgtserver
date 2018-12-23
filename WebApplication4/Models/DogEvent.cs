using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogsServer.Models
{
    public class DogEvent
    {
        public int DogId { get; set; }
        public Dog Dog { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
