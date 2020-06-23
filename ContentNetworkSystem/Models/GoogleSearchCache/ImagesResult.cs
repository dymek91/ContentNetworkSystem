using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentNetworkSystem.Data;
using Newtonsoft.Json;

namespace ContentNetworkSystem.Models.GoogleSearchCache
{
    public class ImagesResult
    {
        public int ID { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public int NicheId { get; set; }

        [JsonIgnore]
        public Niche Niche { get; set; }

    }
}
