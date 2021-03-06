using System.ComponentModel.DataAnnotations;
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

        //public virtual Task PushContent(IServiceProvider serviceProvider, IHttpClientFactory clientFactory)
        //{
        //    throw new NotImplementedException();
        //}
        //public virtual void EncryptPassword(IServiceProvider serviceProvider)
        //{
        //    throw new NotImplementedException();
        //}

    }
}
