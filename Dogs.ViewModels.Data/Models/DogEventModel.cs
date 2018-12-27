using System;
using System.Collections.Generic;
using System.Text;

namespace Dogs.ViewModels.Data.Models
{
    public class DogEventModel
    {
        public int DogId { get; set; }
        public DogModel Dog { get; set; }

        public int EventId { get; set; }
        public EventModel Event { get; set; }

        public string LostPerson { get; set; }
        public string DogTrackBlobUrl { get; set; }
        public string LostPersonTrackBlobUrl { get; set; }
        public string Notes { get; set; }
        public string Weather { get; set; }
    }
}
