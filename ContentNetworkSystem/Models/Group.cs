using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContentNetworkSystem.Models
{
    public class Group
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }

        public ICollection<Project> Projects { get; set; }
    }
}
