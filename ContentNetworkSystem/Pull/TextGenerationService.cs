using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ContentNetworkSystem.Pull.Models;
using IdentityModel.Client;
using System.Text.Json;

namespace ContentNetworkSystem.Pull
{
    public class TextGenerationService
    {

        private readonly IHttpClientFactory _clientFactory;
        private IConfiguration Configuration { get; }
        public TextGenerationService(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            Configuration = configuration;
            _clientFactory = clientFactory;
        }

        public async Task<TextJSON> GetText(int categoryId)
        {
            TextJSON textJSON = null;

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
                    ClientSecret = "7FAEB0A9-301F-4425-ADFE-85062F8D419F",

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
    }
}
