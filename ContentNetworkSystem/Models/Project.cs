using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContentNetworkSystem.Models
{
    public class Project
    {
        public int ID { get; set; }
        public int? ContentId { get; set; }
        public int? GroupId { get; set; }
        [Required]
        public string Name { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime DateAdded { get; set; }
        public DateTime Frequency { get; set; }
        public DateTime LastPushed { get; set; }
        public bool Active { get; set; } = true;

        public Content Content { get; set; }
        public Group Group { get; set; }
    }
}
