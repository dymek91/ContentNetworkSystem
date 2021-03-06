using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContentNetworkSystem.Models
{
    public class Project
    {
        public int ID { get; set; } 
        public int? GroupId { get; set; }
        public int? NicheId { get; set; }
        [Required]
        public string Name { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime DateAdded { get; set; }
        public TimeSpan Frequency { get; set; }
        public DateTime LastPushed { get; set; }
        public bool? WasSuccess { get; set; } = true;
        public bool Active { get; set; } = false;

        public Content Content { get; set; }
        public Group Group { get; set; }

        public Niche Niche { get; set; }
    }
}
