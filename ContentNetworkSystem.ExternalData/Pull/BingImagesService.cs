using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ContentNetworkSystem.Models;
using ContentNetworkSystem.Models.GoogleSearchCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks; 
using ContentNetworkSystem.Data.GoogleSearchCache;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch.Models;

namespace ContentNetworkSystem.Pull
{
    public class BingImagesService : IImagesService
    {
        private readonly IHttpClientFactory _clientFactory;
        private IConfiguration Configuration { get; }
        private ILogger<BingImagesService> _logger;
        private List<IConfigurationSection> BingCognitiveServicesApiKeys { get; set; }
        private IImagesResultsService _imagesResultsService;
        public BingImagesService(IHttpClientFactory clientFactory, IConfiguration configuration, IImagesResultsService imagesResultsService, ILogger<BingImagesService> logger)
        { 
            Configuration = configuration;
            _clientFactory = clientFactory;
            _imagesResultsService = imagesResultsService;
            BingCognitiveServicesApiKeys = Configuration.GetSection("BingCognitiveServicesApiKeys").GetChildren().ToList();
        }
        public async Task<List<ImagesResult>> SearchImages_Api(Niche niche, int imagesToGet)
        {
            var googleImages = new List<ImagesResult>();

            var rand = new Random();

            //CHECK IN CACHE
            if (niche.ImagesResults.Count() >= imagesToGet)
            {
                Console.WriteLine("getting images from cache");
                Console.WriteLine("images count: " + niche.ImagesResults.Count().ToString());
                googleImages = niche.ImagesResults.ToList().GetRange(0, imagesToGet);
                await _imagesResultsService.DeleteRangeAsync(googleImages);
                return googleImages;
            }

            Console.WriteLine("getting images from bing api");

            var keywordsShuffled = niche.Keywords.OrderBy(a => Guid.NewGuid()).ToList();
            string searchTerm = keywordsShuffled[0].Name;

            int keyIndex = rand.Next(0, BingCognitiveServicesApiKeys.Count);

            string bingCSApiKey = BingCognitiveServicesApiKeys[keyIndex].GetValue<string>("ApiKey");

            var client = new ImageSearchClient(new ApiKeyServiceClientCredentials(bingCSApiKey));

            var imageResults = await client.Images.SearchAsync(query: searchTerm, count: 150); //search query

            if (imageResults != null)
            {
                foreach (var searchResult in imageResults.Value)
                {
                    googleImages.Add(new ImagesResult() { Url = searchResult.ContentUrl, Title = searchResult.Name, NicheId = niche.ID, Niche = niche });
                }
            }

            //SAVE TO CACHE
            if (googleImages.Count() > 0)
            {
                googleImages = googleImages.OrderBy(a => Guid.NewGuid()).ToList();
                await _imagesResultsService.AddRangeAsync(googleImages);

                if (googleImages.Count >= imagesToGet)
                {
                    googleImages = googleImages.GetRange(0, imagesToGet);
                    await _imagesResultsService.DeleteRangeAsync(googleImages);
                }
                else
                {
                    await _imagesResultsService.DeleteRangeAsync(googleImages);
                }
            }

            return googleImages;
        }
    }
}
