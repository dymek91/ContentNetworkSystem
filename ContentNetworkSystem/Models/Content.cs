using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Http;

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

        public Project Project { get; set; }

        public abstract void PushContent(IServiceProvider serviceProvider, IHttpClientFactory clientFactory);

    }
}
