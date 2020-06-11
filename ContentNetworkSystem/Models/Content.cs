using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Http;
//using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ContentNetworkSystem.Models
{
    public abstract class Content
    {
        public int ID { get; set; }
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Url { get; set; }
        public string TypeName { get; set; }

        [JsonIgnore]
        public Project Project { get; set; }

        public abstract Task PushContent(IServiceProvider serviceProvider, IHttpClientFactory clientFactory);
        public abstract void EncryptPassword(EncryptionService encryptionService);

    }
}
