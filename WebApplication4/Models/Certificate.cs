using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogsServer.Models
{
    public class Certificate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Level { get; set; }
        public string Description { get; set; }
        public TimeSpan ValidThrough { get; set; }
    }
}
