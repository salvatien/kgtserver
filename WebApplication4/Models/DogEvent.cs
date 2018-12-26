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

        public string LostPerson { get; set; }
        public string DogTrackBlobUrl { get; set; }
        public string LostPersonTrackBlobUrl { get; set; }
        public string Notes { get; set; }
        public string Weather { get; set; }
    }
}
