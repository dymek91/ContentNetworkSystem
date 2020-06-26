using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using IdentityModel.Client;
using Newtonsoft.Json;

namespace ContentNetworkSystem.Client
{
    public class CNSClient
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public string Host { get; set; }
        public string AuthServerHost { get; set; }
        public string LastError { get; set; }
        public string AuthLog { get; set; }

        private string Token { get; set; }

        public CNSClient()
        {

        }

        private TokenResponse Authorize()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var disco = client.GetDiscoveryDocumentAsync(AuthServerHost).GetAwaiter().GetResult();
                    if (disco.IsError)
                    {
                        //Console.WriteLine(disco.Error);
                        // _logger.LogWarning(disco.Error);
                        // project.SendWarningToLog(disco.Error, "Authorize error: ", true);
                        AuthLog = disco.Error;
                        return null;
                    }

                    // request token
                    TokenResponse tokenResponse = client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                    {
                        Address = disco.TokenEndpoint,
                        ClientId = ClientId,
                        ClientSecret = ClientSecret,

                        Scope = Scope
                    }).GetAwaiter().GetResult();

                    return tokenResponse;
                }
            }
            catch(Exception e)
            {
                LastError = e.ToString();
                return null;
            }
        }

        public T Get<T>()
        {
            T obj = default;
            try
            {
                using (var client = new HttpClient())
                {
                    var token = Authorize();
                    client.SetBearerToken(token.AccessToken);
                    HttpResponseMessage response = client.GetAsync(Host + "/api/" + typeof(T).Name + "s").GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        string respJson = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        JsonSerializerSettings settings = new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            TypeNameHandling = TypeNameHandling.Objects
                            //ContractResolver = new CamelCasePropertyNamesContractResolver(),
                            // SerializationBinder = new MyCustomSerializationBinder()
                        };
                        obj = JsonConvert.DeserializeObject<T>(respJson, settings);
                        //obj = await response.Content.ReadAsAsync<T>().GetAwaiter().GetResult();
                    }
                }
            }
            catch(Exception e)
            {
                LastError = e.ToString();
            }
            return obj;
        }
        public T Get<T>(int id)
        {
            T obj = default;
            try
            {
                using (var client = new HttpClient())
                {
                    var token = Authorize();
                    client.SetBearerToken(token.AccessToken);
                    HttpResponseMessage response = client.GetAsync(Host + "/api/" + typeof(T).Name + "s/" + id.ToString()).GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    { 
                        string respJson = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        JsonSerializerSettings settings = new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            TypeNameHandling = TypeNameHandling.Objects
                        };
                        obj = JsonConvert.DeserializeObject<T>(respJson, settings);
                    }
                }
            }
            catch(Exception e)
            {
                LastError = e.ToString();
            }
            return obj;
        }
        public T Get<T>(int id, string functionName)
        {
            T obj = default;
            try
            {
                using (var client = new HttpClient())
                {
                    var token = Authorize();
                    client.SetBearerToken(token.AccessToken);
                    HttpResponseMessage response = client.GetAsync(Host + "/api/" + typeof(T).Name + "s/" + id.ToString() + "/" + functionName).GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        string respJson = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        JsonSerializerSettings settings = new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            TypeNameHandling = TypeNameHandling.Objects
                        };
                        obj = JsonConvert.DeserializeObject<T>(respJson, settings);
                    }
                }
            }
            catch(Exception e)
            {
                LastError = e.ToString();
            }
            return obj;
        }
        public T Post<T>(T obj)
        {
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.Objects,
                    //ContractResolver = new CamelCasePropertyNamesContractResolver(), 
                };
                var jsonString = JsonConvert.SerializeObject(obj, settings);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    var token = Authorize();
                    client.SetBearerToken(token.AccessToken);

                    HttpResponseMessage result = client.PostAsync(Host + "/api/" + typeof(T).Name + "s", content).GetAwaiter().GetResult();
                    var respJson = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    JsonSerializerSettings settingsResp = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        TypeNameHandling = TypeNameHandling.Objects
                    };
                    return JsonConvert.DeserializeObject<T>(respJson, settingsResp);
                }
            }
            catch(Exception e)
            {
                LastError = e.ToString();
                return default(T);
            }
        }
    }
}
