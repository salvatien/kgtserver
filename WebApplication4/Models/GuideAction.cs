﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DogsServer.Models
{
    public class GuideAction
    {
        [ForeignKey("Guide")]
        public int GuideId { get; set; }
        public Guide Guide { get; set; }
        [ForeignKey("Action")]
        public int ActionId { get; set; }
        public Action Action { get; set; }
    }
}