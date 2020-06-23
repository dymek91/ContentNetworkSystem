using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using WordPressSharp;
using WordPressSharp.Models;
//using POSSIBLE.WordPress.XmlRpcClient;
//using POSSIBLE.WordPress.XmlRpcClient.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using System.Drawing;
using System.IO;

namespace ContentNetworkSystem.Push
{
    public class WordpressService
    {
        private ILogger<WordpressService> _logger;
        public WordpressService(ILogger<WordpressService> logger)
        {
            _logger = logger; 
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

        public string PushPost(string user, string pass, string baseUrl, int blogId, string postTitle, string postContent)
        {

            var config = new WordPressSiteConfig
            {
                BaseUrl = baseUrl,
                BlogId = blogId,
                Username = user,
                Password = pass
            };

            string postId = "-1";
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

                postId = client.NewPost(post);
            }
            return postId;
        }

        public string UploadImage(string user, string pass, string baseUrl, int blogId, string imageUrl,int maxWidth,int maxHeight)
        {
            var config = new WordPressSiteConfig
            {
                BaseUrl = baseUrl,
                BlogId = blogId,
                Username = user,
                Password = pass
            };

            string Id = "-1";
            try
            {
                using (var client = new WordPressClient(config))
                {
                    var data = WordPressSharp.Models.Data.CreateFromUrl(imageUrl);
                    data.Name = FixFileName(data.Name);
                    data.Bits = ResizeImage(data.Bits, data.Type, maxWidth, maxHeight);
                    Id = client.UploadFile(data).Id;
                }
            }
            catch(Exception e)
            {
                //_logger.LogError("Message: {Message} | StackTrace: {StackTrace}", e.Message, e.StackTrace);
                _logger.LogError("{0}",e);
            }
            return Id;
        }

        public bool EditPostThumbnail(string user, string pass, string baseUrl, int blogId,string postId, string imageId)
        {
            var config = new WordPressSiteConfig
            {
                BaseUrl = baseUrl,
                BlogId = blogId,
                Username = user,
                Password = pass
            };
            bool success = false;
            using (var client = new WordPressClient(config))
            {
                var post = new Post
                {
                    Id = postId,
                    FeaturedImageId = imageId
                };
                success = client.EditPost(post);
            }
            return success;
        }

        private string FixFileName(string fileName)
        {
            fileName = fileName.Split('?')[0];
            fileName = fileName.Split(';')[0];
            return fileName;
        }

        private byte[] ResizeImage(byte[] imgBytes, string mimeType, double maxWidth, double maxHeight)
        {
            Image imgToResize; 
            using (var ms = new MemoryStream(imgBytes))
            {
                imgToResize = new Bitmap(ms);
            } 
            double resizeWidth = imgToResize.Width;
            double resizeHeight = imgToResize.Height;

            double aspect = resizeWidth / resizeHeight;

            if (resizeWidth > maxWidth)
            {
                resizeWidth = maxWidth;
                resizeHeight = resizeWidth / aspect;
            }
            if (resizeHeight > maxHeight)
            {
                aspect = resizeWidth / resizeHeight;
                resizeHeight = maxHeight;
                resizeWidth = resizeHeight * aspect;
            }

            //Image imgResized = (Image)(new Bitmap(imgToResize, new Size((int) resizeWidth, (int)resizeHeight)));  
            Image imgResized = imgToResize.GetThumbnailImage((int)resizeWidth, (int)resizeHeight, null, IntPtr.Zero); 
            using (var ms = new MemoryStream())
            {
                imgResized.Save(ms, imgToResize.RawFormat);
                return ms.ToArray();
            }
        }

    }
}
