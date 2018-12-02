using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace kgtwebClient.Models
{
    public class Action
    {
        public int ActionID { get; set; }
        public DateTime Date { get; set; }
        public string City { get; set; }
        public string StreetOrLocation { get; set; }
        public string LostPersonFirstName { get; set; }
        public string LostPersonLastName { get; set; }
        public string Coordinator { get; set; }
        public bool WasSuccess { get; set; }
        public string Notes { get; set; }
        public virtual IList<GuideAction> GuideActions { get; set; }
    }
}
