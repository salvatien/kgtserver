using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kgtwebClient.Models
{
    public class GuideActionModel
    {
        public int GuideId { get; set; }
        public GuideModel Guide { get; set; }

        public int ActionId { get; set; }
        public ActionModel Action { get; set; }
    }
}
