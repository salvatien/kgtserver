using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kgtwebClient.Models
{
    public class GuideAction
    {
        public int GuideId { get; set; }
        public Guide Guide { get; set; }

        public int ActionId { get; set; }
        public Action Action { get; set; }
    }
}
