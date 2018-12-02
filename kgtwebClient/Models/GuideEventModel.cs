using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kgtwebClient.Models
{
    public class GuideEventModel
    {
        public int GuideId { get; set; }
        public GuideModel Guide { get; set; }

        public int EventId { get; set; }
        public EventModel Event { get; set; }
    }
}
