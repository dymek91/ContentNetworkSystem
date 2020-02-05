using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentNetworkSystem.Models
{
    public class Project
    {
        public int ID { get; set; }
        public int ContentId { get; set; }
        public int? GroupId { get; set; }
        public string Name { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime Frequency { get; set; }
        public DateTime LastPushed { get; set; }
        public bool Active { get; set; }
    }
}
