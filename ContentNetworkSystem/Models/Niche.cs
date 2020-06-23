using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using ContentNetworkSystem.Models.GoogleSearchCache;

namespace ContentNetworkSystem.Models
{
    public class Niche
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<Project> Projects { get; set; }
        public ICollection<Keyword> Keywords { get; set; }
        [JsonIgnore]
        public ICollection<YoutubeResult> YoutubeResults { get; set; }
        [JsonIgnore]
        public ICollection<ImagesResult> ImagesResults { get; set; }
    }
}
