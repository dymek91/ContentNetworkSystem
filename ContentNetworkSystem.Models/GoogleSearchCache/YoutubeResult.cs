using Newtonsoft.Json;

namespace ContentNetworkSystem.Models.GoogleSearchCache
{
    public class YoutubeResult
    {
        public int ID { get; set; }
        public string VideoId { get; set; }
        public string Title { get; set; }
        public int NicheId { get; set; }

        [JsonIgnore]
        public Niche Niche { get; set; }
    }
}
