using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WordPressSharp;
using WordPressSharp.Models;
//using POSSIBLE.WordPress.XmlRpcClient;
//using POSSIBLE.WordPress.XmlRpcClient.Models;

namespace ContentNetworkSystem.Push
{
    public class WordpressService
    { 
        public WordpressService()
        {

        }

        //public void PushPost(string user, string pass, string baseUrl, int blogId, string postTitle, string postContent)
        //{
        //    var config = new WordPressSiteConfig
        //    {
        //        BaseUrl = baseUrl,
        //        BlogId = blogId,
        //        Username = user,
        //        Password = pass
        //    };

        //    using (var client = new WordPressClient(config))
        //    {
        //        var post = new Post
        //        {
        //            PostType = "post",
        //            Title = postTitle,
        //            Content = postContent,
        //            PublishDateTime = DateTime.Now
        //        };

        //        var id = Convert.ToInt32(client.NewPost(post));
        //    }
        //}

        //public void PushPost(string user, string pass, string baseUrl, int blogId, string postTitle, string postContent)
        //{ 

        //    using (var client = new WordPressXmlRpcClient(baseUrl,user,pass,blogId))
        //    {
        //        var post = new Post
        //        { 
        //            post_type = "post",
        //            post_title = postTitle,
        //            post_content = postContent,
        //            post_date = DateTime.Now
        //        };

        //        var id = Convert.ToInt32(client.NewPost(post)); 
        //    }
        //}

        public void PushPost(string user, string pass, string baseUrl, int blogId, string postTitle, string postContent)
        {

            var config = new WordPressSiteConfig
            {
                BaseUrl = baseUrl,
                BlogId = blogId,
                Username = user,
                Password = pass
            };

            using (var client = new WordPressClient(config))
            {
                var post = new Post
                {
                    Status = "publish",
                    PostType = "post",
                    Title = postTitle,
                    Content = postContent,
                    PublishDateTime = DateTime.UtcNow
                };

                var id = Convert.ToInt32(client.NewPost(post));
            }
        }
    }
}
