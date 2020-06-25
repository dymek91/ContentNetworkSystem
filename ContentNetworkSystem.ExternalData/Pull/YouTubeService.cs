using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Localization.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3.Data;
using ContentNetworkSystem.Models.GoogleSearchCache;
using ContentNetworkSystem.Models;
using ContentNetworkSystem.Data.GoogleSearchCache;

namespace ContentNetworkSystem.Pull
{
    public class YouTubeService
    {
        private readonly IHttpClientFactory _clientFactory;
        private IConfiguration Configuration { get; } 
        private string ProxiesListUrl { get; }
        private string VideoSearchUrl { get; } = "https://www.youtube.com/results?filters=video&search_query=%search%&lclk=video";
        private List<Proxy> Proxies { get; set; }
        private ILogger<YouTubeService> _logger;
        private List<IConfigurationSection> GoogleApiKeys { get; set; }
        private IYoutubeResultsService _youtubeResultsService;
        public YouTubeService(IHttpClientFactory clientFactory, IConfiguration configuration,IYoutubeResultsService youtubeResultsService, ILogger<YouTubeService> logger)
        {
            Configuration = configuration;
            _clientFactory = clientFactory;
            _youtubeResultsService = youtubeResultsService;
            ProxiesListUrl = Configuration.GetValue<string>("ProxiesListUrl");
            GoogleApiKeys = Configuration.GetSection("GoogleApiKeys").GetChildren().ToList();
        }

        public async Task<List<YoutubeResult>> SearchVideos_Parse(Niche niche, int videosToGet)
        {
            LoadProxies().GetAwaiter().GetResult();

            var rand = new Random();
            var proxy = Proxies[rand.Next(0,Proxies.Count())];

            var keywordsShuffled = niche.Keywords.OrderBy(a => Guid.NewGuid()).ToList();
            string searchTerm = keywordsShuffled[0].Name;

            var webProxy = new WebProxy
            {
                Address = new Uri($"http://{proxy.Host}:{proxy.Port}"),
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false,

                // *** These creds are given to the proxy server, not the web server ***
                Credentials = new NetworkCredential(
                userName: proxy.User,
                password: proxy.Password)
            };

            // Now create a client handler which uses that proxy
            var httpClientHandler = new HttpClientHandler
            {
                Proxy = webProxy,
            };

            // Omit this part if you don't need to authenticate with the web server:
            bool needServerAuthentication = true;
            if (needServerAuthentication)
            {
                httpClientHandler.PreAuthenticate = true;
                httpClientHandler.UseDefaultCredentials = false;

                // *** These creds are given to the web server, not the proxy server ***
                httpClientHandler.Credentials = new NetworkCredential(
                    userName: proxy.User,
                    password: proxy.Password);
            }

            var ytVideos = new List<YoutubeResult>();
            using (var client = new HttpClient(handler: httpClientHandler, disposeHandler: true))
            {
                client.Timeout = TimeSpan.FromSeconds(10);
                string videoSearchUrl = VideoSearchUrl.Replace("%search%", searchTerm);
                string getResult = await client.GetStringAsync(videoSearchUrl);
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(getResult);
                var nodes = htmlDocument.DocumentNode.SelectNodes("//h3[@class='yt-lockup-title ']");
                if (nodes == null)
                {
                    return ytVideos;
                }
                var shuffledNodes = nodes.OrderBy(a => Guid.NewGuid()).ToList();

                int iter = 0;
                foreach(var node in shuffledNodes)
                {
                    var aNode = node.ChildNodes[0];
                    var videoId = aNode.Attributes["href"].Value.Split('=')[1];
                    var title = aNode.InnerText;

                    ytVideos.Add(new YoutubeResult { VideoId = videoId,Title = title, NicheId = niche.ID, Niche = niche});

                    iter++;
                    if (iter >= videosToGet) break;
                } 
            }
            return ytVideos;
        }
        public async Task<List<YoutubeResult>> SearchVideos_Api(Niche niche, int videosToGet)
        {
            var ytVideos = new List<YoutubeResult>();

            var rand = new Random();

            //CHECK IN CACHE
            if(niche.YoutubeResults.Count() >= videosToGet)
            {
                Console.WriteLine("getting videos from cache");
                Console.WriteLine("videos count: " + niche.YoutubeResults.Count().ToString());
                ytVideos = niche.YoutubeResults.ToList().GetRange(0, videosToGet);
                await _youtubeResultsService.DeleteRangeAsync(ytVideos);
                return ytVideos;
            }
             
            Console.WriteLine("getting videos from google api");

            var keywordsShuffled = niche.Keywords.OrderBy(a => Guid.NewGuid()).ToList();
            string searchTerm = keywordsShuffled[0].Name;

            string googleApiKey = GoogleApiKeys[rand.Next(0, GoogleApiKeys.Count)].GetValue<string>("ApiKey");

            var youtubeService = new Google.Apis.YouTube.v3.YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = googleApiKey,
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = searchTerm; // Replace with your search term.
            searchListRequest.MaxResults = 20;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            // Add each result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.
            foreach (var searchResult in searchListResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video": 
                        ytVideos.Add(new YoutubeResult() { VideoId= searchResult.Id.VideoId, Title= searchResult.Snippet.Title, NicheId = niche.ID, Niche = niche });
                        break; 
                }
            }

            //SAVE TO CACHE
            if (ytVideos.Count()>0)
            {
                ytVideos = ytVideos.OrderBy(a => Guid.NewGuid()).ToList();
                await _youtubeResultsService.AddRangeAsync(ytVideos);

                if (ytVideos.Count >= videosToGet)
                {
                    ytVideos = ytVideos.GetRange(0, videosToGet);
                    await _youtubeResultsService.DeleteRangeAsync(ytVideos);
                }
                else
                {
                    await _youtubeResultsService.DeleteRangeAsync(ytVideos);
                }
            }

            return ytVideos;
        }


        public async Task<List<string>> SearchVideosIFrame(Niche niche, int videosToGet)
        {
            var rand = new Random();

            string[] styleFLoat = { "#vleft left", "#vleft left", "#vleft left", "#vleft left", "#vright right" };
            string[] stylePadding = { "#vright 10px 0px 10px 10px", "#vleft 10px 10px 10px 0px" };
            string iframeTemplate = $"<iframe width=\"640\" height=\"360\" src=\"//www.youtube.com/embed/%content%\" frameborder=\"0\" allowfullscreen title=\"%title% (c)\" style=\"float:{styleFLoat[rand.Next(0,styleFLoat.Count())]};padding:{stylePadding[rand.Next(0,stylePadding.Count())]};border:0px;\"></iframe>";
            var ytVideos = await SearchVideos_Api(niche, videosToGet);

            List<string> iframes = new List<string>();

            foreach(var ytVideo in ytVideos)
            {
                string iframe = iframeTemplate.Replace("%content%", ytVideo.VideoId);
                iframe = iframe.Replace("%title%", ytVideo.Title);
                iframes.Add(iframe);
            }

            return iframes;
        }

        private async Task LoadProxies()
        {
            Proxies = new List<Proxy>();
            using (var client = _clientFactory.CreateClient())
            {
                var proxiesList = await client.GetStringAsync(ProxiesListUrl);
                using (StringReader sr = new StringReader(proxiesList))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        Proxies.Add(new Proxy(line));
                    }
                } 
            }
        }

        private class Proxy
        {
            public string Host { get; set; }
            public string Port { get; set; }
            public string User { get; set; }
            public string Password { get; set; }
            public Proxy(string proxyLine)
            { 
                string[] data = proxyLine.Trim().Split(':');
                Host = data[0];
                Port = data[1];
                User = data[2];
                Password = data[3];
            }
        }
    } 
}
