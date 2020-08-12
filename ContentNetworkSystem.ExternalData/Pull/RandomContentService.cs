using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ContentNetworkSystem.Models;
using ContentNetworkSystem.Models.GoogleSearchCache;

namespace ContentNetworkSystem.Pull
{
    public class RandomContentService
    {

        private readonly IHttpClientFactory _clientFactory;
        private IServiceProvider _serviceProvider;
        private IConfiguration Configuration { get; }
        private string ContentSearchFilePath { get; }
        private string AurthorityLinksFilePath { get; }
        private List<string> AuthorityLinks { get; }
        public RandomContentService(IHttpClientFactory clientFactory, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            Configuration = configuration;
            _clientFactory = clientFactory;
            _serviceProvider = serviceProvider;

            string ContentSearchFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Configuration.GetValue<string>("ContentSearch"));
            string AurthorityLinksFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Configuration.GetValue<string>("AuthorityLinks"));

            AuthorityLinks = File.ReadAllLines(AurthorityLinksFilePath).ToList();
        }  

        public string InsertLinksToText(string text, int linksToAddCount)
        {
            var indexesOfSpaces = text.AllIndexesOf(' ');
            indexesOfSpaces = indexesOfSpaces.OrderBy(e => Guid.NewGuid()).ToList()
                .GetRange(0, linksToAddCount < indexesOfSpaces.Count() ? linksToAddCount : indexesOfSpaces.Count())
                .OrderByDescending(e => e);

            foreach (var indexOfSpace in indexesOfSpaces)
            {
                int indexOfWord = indexOfSpace + 1;
                var word = text.Substring(indexOfWord, text.IndexOf(" ", indexOfWord) == -1 ? text.Length - indexOfWord : text.IndexOf(" ", indexOfWord) - indexOfWord);
                if (word.Length == 0) continue;
                string authorityLink = GetRandomAuthorityLink().Replace("%anchor_text%",word);
                string aTag = $"<a href=\"{authorityLink}\">{word}</a>";
                text = text.Remove(indexOfWord, word.Length);
                text = text.Insert(indexOfWord, aTag);
            }

            return text;
        }

        public async Task<string> InsertVideosToText(Niche niche, string text, int videosToAdd)
        {
            var rand = new Random();

            var youtubeService = _serviceProvider.GetService<YouTubeService>();

            var iframes = await youtubeService.SearchVideosIFrame(niche, videosToAdd);
            foreach (var iframe in iframes)
            {
                var positions = GetAvailablePositions(text);
                text = text.Insert(positions[rand.Next(0, positions.Count())], '\n' + iframe);
            }

            return text;
        }

        public async Task<string> InsertImagesToText(Niche niche, string text, int imagesToAdd)
        {
            var rand = new Random();

            var googleImagesService = _serviceProvider.GetService<GoogleImagesService>();
            var bingImagesService = _serviceProvider.GetService<BingImagesService>();
             
            List<IImagesService> imagesServices = new List<IImagesService>
            {
                googleImagesService,
                bingImagesService
            };
            IImagesService imagesService = imagesServices[rand.Next(0, imagesServices.Count)];

            var googleImages = await imagesService.SearchImages_Api(niche, imagesToAdd);
            if (googleImages.Count() == 0)
            {
                return text;
            }
             
            var imgTags = CreateImgTags(googleImages);
            foreach(var imgTag in imgTags)
            {
                var positions = GetAvailablePositions(text);
                text = text.Insert(positions[rand.Next(0, positions.Count())], '\n' + imgTag);
            }

            return text;
        }

        public string GetRandomAuthorityLink()
        {
            var rand = new Random();
            return AuthorityLinks[rand.Next(0, AuthorityLinks.Count)];
        }

        public enum ContentSearchTypes
        {
            CONTENT_SEARCH_VIDEO,
            CONTENT_SEARCH_IMAGE
        } 
        private List<int> GetAvailablePositions(string postContent)
        {
            var positions = postContent.AllIndexesOf('\n').ToList();
            if (positions.Count() > 0)
            {
                positions = positions.OrderBy(a => Guid.NewGuid()).ToList();
            }
            else
            {
                positions.Add(postContent.Length);
            }
            return positions;
        }

        private List<string> CreateImgTags(List<ImagesResult> images)
        {
            var imgTags = new List<string>();
            var rand = new Random();
            string[] maxWidth_options = { "1", "2", "3", "4", "5", "0", "0", "0" };
            string[] imgFloat_options = { "#left left", "#left left", "#left left", "#left left", "#right right" };
            string[] imgPadding_options = { "#right 10px 0px 10px 10px", "#left 10px 10px 10px 0px" };
            string maxWidth = maxWidth_options[rand.Next(0, maxWidth_options.Count())];
            string imgFloat = imgFloat_options[rand.Next(0, imgFloat_options.Count())];
            string imgPadding = imgPadding_options[rand.Next(0, imgPadding_options.Count())];
            foreach (var image in images)
            {
                string template = $"<img src=\"{image.Url}\" alt=\"{image.Title}\" style=\"max-width:4{maxWidth}0px;float:{imgFloat};padding:{imgPadding};border:0px;\">";
                imgTags.Add(template);
            }
            return imgTags;
        }

        private class  ContentSearch
        {
            public string Name { get; set; }
            public string Url { get; set; }
            public string Type { get; set; }
            public string Separator { get; set; }
            public string[] ContentFront { get; set; }
            public string[] ContentBack { get; set; }
            public string ContentReplace { get; set; }
            public string[] AuthorFront { get; set; }
            public string[] AuthorBack { get; set; }
            public string[] TitleFront { get; set; }
            public string[] TitleBack { get; set; }
            public string[] Result { get; set; }
            public string MustHave { get; set; }

        }
    }
}
