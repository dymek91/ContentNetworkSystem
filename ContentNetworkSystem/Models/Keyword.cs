using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ContentNetworkSystem.Models
{
    public class Keyword
    { 
        public int ID { get; set; }
        [Required]
        public int NicheId { get; set; }
        [Required]
        public string Name { get; set; }

        [JsonIgnore]
        public Niche Niche { get; set; }
    }
}
