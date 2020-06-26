using System.ComponentModel.DataAnnotations;
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
