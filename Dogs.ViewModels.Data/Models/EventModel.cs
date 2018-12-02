using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogs.ViewModels.Data.Models
{
    public class EventModel
    {
        public int EventID { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string City { get; set; }
        public string StreetOrLocation { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public List<string> GuideEventIds { get; set; }
        public List<string> DogIds { get; set; }
    }
}
