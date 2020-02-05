using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentNetworkSystem.Models
{
    public class Group
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public ICollection<Project> Projects { get; set; }
    }
}
