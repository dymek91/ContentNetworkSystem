
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ContentNetworkSystem.Push;
using ContentNetworkSystem.Pull;
using ContentNetworkSystem.Pull.Models;

namespace ContentNetworkSystem.Models
{
    public class Wordpress : Content
    { 
        public int? TextGenerationCategoryId { get; set; }
        public int? BlogId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public override void EncryptPassword(EncryptionService encryptionService)
        {
            Password = encryptionService.EncryptString(Password);
        }

        public override async  Task PushContent(IServiceProvider serviceProvider, IHttpClientFactory clientFactory)
        { 
            Console.WriteLine("Pushing wordpress content");

            int blogId = 1;
            if (BlogId.HasValue) { blogId = BlogId.Value; }

            var wordpressService =  serviceProvider.GetService<WordpressService>();
            var textGenerationService = serviceProvider.GetService<TextGenerationService>();
            var encryptionService = serviceProvider.GetService<EncryptionService>();

            TextJSON textJSON = await textGenerationService.GetText(TextGenerationCategoryId.Value);
            string postTitle = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(textJSON.SuggestedTitle));  
            string postContent= System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(textJSON.Text));
            string password = encryptionService.DecryptString(Password);

            wordpressService.PushPost(
                user: Username,
                pass: password,
                baseUrl: Url,
                blogId:blogId,
                postTitle:postTitle,
                postContent: postContent);

        }
    }
}
