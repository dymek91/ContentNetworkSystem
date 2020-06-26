using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
