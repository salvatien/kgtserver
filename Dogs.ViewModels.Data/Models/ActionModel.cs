using System;
using System.Collections.Generic;

namespace Dogs.ViewModels.Data.Models
{
    public class ActionModel
    {
        public int ActionId { get; set; }
        public DateTime Date { get; set; }
        public string City { get; set; }
        public string StreetOrLocation { get; set; }
        public string LostPersonFirstName { get; set; }
        public string LostPersonLastName { get; set; }
        public string Coordinator { get; set; }
        public bool WasSuccess { get; set; }
        public string Notes { get; set; }
        public virtual IList<GuideActionModel> GuideActions { get; set; }
    }
}
