namespace Dogs.Data.Models
{
    public class GuideEvent
    {
        public int GuideId { get; set; }
        public Guide Guide { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}

