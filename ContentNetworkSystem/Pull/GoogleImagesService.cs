using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Apis;
using Google.Apis.Services;
using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;
using ContentNetworkSystem.Models;
using ContentNetworkSystem.Models.GoogleSearchCache;
using ContentNetworkSystem.Data.GoogleSearchCache;

namespace ContentNetworkSystem.Pull
{
    public class GoogleImagesService
    {
        private readonly IHttpClientFactory _clientFactory;
        private IConfiguration Configuration { get; }  
        private ILogger<GoogleImagesService> _logger; 
        private List<IConfigurationSection> GoogleApiKeys { get; set; }
        private IImagesResultsService _imagesResultsService;
        
        public GoogleImagesService(IHttpClientFactory clientFactory, IConfiguration configuration, IImagesResultsService imagesResultsService, ILogger<GoogleImagesService> logger)
        {
            Configuration = configuration;
            _clientFactory = clientFactory;
            _imagesResultsService = imagesResultsService;
            GoogleApiKeys = Configuration.GetSection("GoogleApiKeys").GetChildren().ToList();
        }

        public async Task<List<ImagesResult>> SearchImages_Api(Niche niche, int imagesToGet)
        {
            var googleImages = new List<ImagesResult>(); 

            var rand = new Random();

            //CHECK IN CACHE
            if (niche.ImagesResults.Count() >= imagesToGet)
            {
                Console.WriteLine("getting images from cache");
                Console.WriteLine("images count: "+ niche.ImagesResults.Count().ToString());
                googleImages = niche.ImagesResults.ToList().GetRange(0, imagesToGet);
                await _imagesResultsService.DeleteRangeAsync(googleImages);
                return googleImages;
            }

            Console.WriteLine("getting images from google api");
             
            var keywordsShuffled = niche.Keywords.OrderBy(a => Guid.NewGuid()).ToList();
            string searchTerm = keywordsShuffled[0].Name;

            int keyIndex = rand.Next(0, GoogleApiKeys.Count);

            string googleApiKey = GoogleApiKeys[keyIndex].GetValue<string>("ApiKey");
            string cx = GoogleApiKeys[keyIndex].GetValue<string>("cx");

            var imagesService = new CustomsearchService(new BaseClientService.Initializer()
            {
                ApiKey = googleApiKey,
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = imagesService.Cse.List(); 
            searchListRequest.Q = searchTerm+ " inurl:jpeg|jpg"; // Replace with your search term.
            searchListRequest.Num = 10;
            searchListRequest.Cx = cx;
            searchListRequest.SearchType = CseResource.ListRequest.SearchTypeEnum.Image;
            searchListRequest.FileType = "jpg"; 

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();
             
            foreach (var searchResult in searchListResponse.Items)
            {
                googleImages.Add(new ImagesResult() { Url=searchResult.Link,Title=searchResult.Title, NicheId=niche.ID, Niche = niche});
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
