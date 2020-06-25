using System;
using System.Collections.Generic;
using System.Text;
using ContentNetworkSystem.Models;
using System.Threading.Tasks;
using System.Net.Http;

namespace ContentNetworkSystem.ModelsExtensions
{
    public static class ModelsExtensions
    {
        public static void EncryptPassword(this Content content, IServiceProvider serviceProvider)
        {
            if (content.GetType() == typeof(Wordpress))
            {
                WordpressExtension.EncryptPassword((Wordpress)content, serviceProvider);
            }
        }
        public static async Task PushContent(this Content content, IServiceProvider serviceProvider, IHttpClientFactory clientFactory)
        {
            if (content.GetType() == typeof(Wordpress))
            {
                await WordpressExtension.PushContent((Wordpress)content, serviceProvider, clientFactory);
            }
        }
    }
}
