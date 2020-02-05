using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentNetworkSystem.Models
{
    public abstract class Content
    {
        public int ID { get; set; }
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string TypeName { get; set; }

        public Project Project { get; set; }

        public abstract void PushContent();

    }
}
