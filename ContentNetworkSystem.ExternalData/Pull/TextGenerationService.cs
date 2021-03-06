using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ContentNetworkSystem.Pull.Models;
using IdentityModel.Client;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ContentNetworkSystem.Pull
{
    public class TextGenerationService
    {

        private readonly IHttpClientFactory _clientFactory;
        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Env { get; }
        public TextGenerationService(IHttpClientFactory clientFactory, IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
            _clientFactory = clientFactory;
        }

        public async Task<TextJSON> GetText(int categoryId)
        {
            TextJSON textJSON = null;

            /////DEBUG/////
            if (Env.IsDevelopment())
            {
                textJSON = new TextJSON
                {
                    SuggestedTitle = "Neque porro quisquam est qui dolorem",
                    Text = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam id augue velit. Curabitur congue dignissim libero, dignissim dapibus justo imperdiet a. Nunc quis dolor a tellus dignissim tincidunt. Pellentesque sit amet sapien ex. Proin rhoncus ullamcorper lacinia. Aliquam erat volutpat. Nulla malesuada enim urna, id consectetur sem lacinia sed. Praesent ex mauris, ultricies in orci ut, gravida egestas augue. Vivamus hendrerit pharetra sapien, quis tincidunt ex sodales in. Donec blandit, arcu faucibus condimentum dapibus, mauris diam sagittis enim, pretium elementum ex risus tincidunt lectus. Cras felis arcu, euismod id tincidunt ac, consectetur congue ex. Fusce in volutpat mi. Nam sit amet elit aliquet, dignissim velit ac, dignissim turpis. Nulla accumsan accumsan venenatis.

Phasellus eu ligula eget neque mattis vulputate.Sed nec ullamcorper justo.Vivamus euismod pharetra magna vitae pharetra.Pellentesque iaculis aliquam purus sit amet molestie.Aenean vel maximus orci.Donec at consectetur eros,
                nec sodales nunc.Curabitur porttitor purus id semper maximus.Nulla facilisis arcu ac leo tincidunt,
                et venenatis libero varius.Donec auctor luctus efficitur.Sed eget vulputate lorem.Aenean vitae tortor nibh.Vestibulum malesuada vestibulum venenatis."
                };
                //textJSON.Text = "dsfsgdfgdfhgdgh";
                textJSON.SuggestedTitle = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(textJSON.SuggestedTitle));
                textJSON.Text = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(textJSON.Text));
                return textJSON;
            }
            /////DEBUG/////-/

            using (var httpClient = _clientFactory.CreateClient())
            {
                var disco = await httpClient.GetDiscoveryDocumentAsync(Configuration.GetValue<string>("AuthorityServer"));
                if (disco.IsError)
                {
                    Console.WriteLine(disco.Error);
                    return null;
                }

                // request token
                var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "CNS",
                    ClientSecret = Configuration.GetValue<string>("ClientSecret"),

                    Scope = "apiTextGeneration"
                });

                if (tokenResponse.IsError)
                {
                    Console.WriteLine(tokenResponse.Error);
                    return null;
                }

                httpClient.SetBearerToken(tokenResponse.AccessToken);
                 
                using (var response = await httpClient.GetStreamAsync(Configuration.GetValue<string>("TextGenerationHost") + "api/texts/" + categoryId.ToString()))
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    textJSON = await JsonSerializer.DeserializeAsync<TextJSON>(response, options);
                }
            }

            return textJSON;
        }
        //id: name
        public async Task<Dictionary<string,string>> GetGenerators()
        {
            using (var httpClient = _clientFactory.CreateClient())
            {
                using (var response = await httpClient.GetStreamAsync(Configuration.GetValue<string>("TextGenerationHost") + "api/generators"))
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    Dictionary<string, string> gensDict = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(response, options);
                    return gensDict;
                }
            }
        }
        //id: <id, name>
        public async Task<Dictionary<string, Dictionary<string, string>>> GetGeneratorsWithCategories()
        {
            using (var httpClient = _clientFactory.CreateClient())
            {
                Dictionary<string, string> gensDict = new Dictionary<string, string>();
                Dictionary<string, Dictionary<string, string>> gensDictWithCats = new Dictionary<string, Dictionary<string, string>>();
                using (var response = await httpClient.GetStreamAsync(Configuration.GetValue<string>("TextGenerationHost") + "api/generators"))
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    gensDict = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(response, options); 
                }
                foreach (var genDict in gensDict)
                {
                    using (var response = await httpClient.GetStreamAsync(Configuration.GetValue<string>("TextGenerationHost") + "api/categories/" + genDict.Key))
                    {
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var genCats = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(response, options);
                        gensDictWithCats.Add(genDict.Key, genCats);
                    }
                }
                return gensDictWithCats;
            }
        }
    }
}
