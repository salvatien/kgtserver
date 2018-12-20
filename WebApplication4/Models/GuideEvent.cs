
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogsServer.Models
{
    public class GuideEvent
    {
        public int GuideId { get; set; }
        public Guide Guide { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}

