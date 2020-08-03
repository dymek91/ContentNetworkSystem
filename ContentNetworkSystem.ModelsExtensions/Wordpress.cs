
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ContentNetworkSystem.Push;
using ContentNetworkSystem.Pull;
using ContentNetworkSystem.Pull.Models;
using Z.EntityFramework.Extensions.Internal;
using ContentNetworkSystem.Models.GoogleSearchCache;
using ContentNetworkSystem.Models;
using Microsoft.EntityFrameworkCore.Internal;

namespace ContentNetworkSystem.ModelsExtensions
{
    public static class WordpressExtension
    {
       
        //public static void EncryptPassword(this Content wordpressCon, IServiceProvider serviceProvider)
        //{
        //    if (wordpressCon.GetType() == typeof(Wordpress))
        //    {
        //        EncryptPassword((Wordpress)wordpressCon, serviceProvider);
        //    }
        //}
        //public static async Task PushContent(this Content wordpressCon, IServiceProvider serviceProvider, IHttpClientFactory clientFactory)
        //{
        //    if (wordpressCon.GetType() == typeof(Wordpress))
        //    {
        //        await PushContent((Wordpress)wordpressCon, serviceProvider, clientFactory);
        //    }
        //}

        public static  void EncryptPassword(Wordpress wordpress, IServiceProvider serviceProvider) 
        {
            //Wordpress wordpress = (Wordpress)wordpressCon;
            EncryptionService encryptionService = serviceProvider.GetService<EncryptionService>();
            wordpress.Password = encryptionService.EncryptString(wordpress.Password);
        }
        //public static async Task PushContent(this Content wordpressCon, IServiceProvider serviceProvider, IHttpClientFactory clientFactory)
        //{
        //    throw new NotImplementedException();
        //}
        public static async  Task PushContent(Wordpress wordpress, IServiceProvider serviceProvider, IHttpClientFactory clientFactory) 
            
        { 
            Console.WriteLine("Pushing wordpress content");

            //Wordpress wordpress = (Wordpress)wordpressCon;

            int blogId = 1;
            if (wordpress.BlogId.HasValue) { blogId = wordpress.BlogId.Value; }

            var wordpressService =  serviceProvider.GetService<WordpressService>();
            var textGenerationService = serviceProvider.GetService<TextGenerationService>();
            var encryptionService = serviceProvider.GetService<EncryptionService>();
            var googleImagesService = serviceProvider.GetService<GoogleImagesService>();
            var randomContentService = serviceProvider.GetService<RandomContentService>();

            TextJSON textJSON = await textGenerationService.GetText(wordpress.TextGenerationCategoryId.Value);
            string postTitle = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(textJSON.SuggestedTitle));  
            string postContent= System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(textJSON.Text));
            string password = encryptionService.DecryptString(wordpress.Password);

            //ADD AUTHORITY LINKS
            int authorityLinksToAdd = GetCountFromMask(wordpress.AuthorityLinksCount);
            if (authorityLinksToAdd > 0)
            {
                postContent = randomContentService.InsertLinksToText(postContent, authorityLinksToAdd);
            } 

            if (wordpress.Project.Niche == null)
            {
                wordpressService.PushPost(
                    user: wordpress.Username,
                    pass: password,
                    baseUrl: wordpress.Url,
                    blogId: blogId,
                    postTitle: postTitle,
                    postContent: postContent);
                return;
            } 

            //ADD VIDEO
            int videosToAdd = GetCountFromMask(wordpress.VideosCount);
            if (videosToAdd>0)
            {
                postContent = await randomContentService.InsertVideosToText(wordpress.Project.Niche, postContent, videosToAdd);
            }

            //ADD IMAGES 
            int imagesToAdd = GetCountFromMask(wordpress.ImagesCount); 
              
            if (imagesToAdd > 0)
            {
                postContent = await randomContentService.InsertImagesToText(wordpress.Project.Niche, postContent, imagesToAdd);
            }

            //ADD KEYWORDS TO TEXT AND TITLE
            string titleKeyword = wordpress.Project.Niche.Keywords.OrderBy(e => Guid.NewGuid()).First().Name;
            titleKeyword = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(titleKeyword);
            string textKeywords = String.Join(", ", wordpress.Project.Niche.Keywords.OrderBy(e => Guid.NewGuid()).Take(5).Select(e => e.Name));
            postTitle = postTitle + " - " + titleKeyword;
            postContent = postContent + @"

" + textKeywords;

            //SEND POST
            var postId = wordpressService.PushPost(
                user: wordpress.Username,
                pass: password,
                baseUrl: wordpress.Url,
                blogId:blogId,
                postTitle:postTitle,
                postContent: postContent);

            //ADD THUMBNAIL
            if(wordpress.AddThumbnail.HasValue)
            {
                if(wordpress.AddThumbnail.Value)
                {
                    var images = await googleImagesService.SearchImages_Api(wordpress.Project.Niche, 1);
                    if (images.Count>0)
                    {
                        var imageThumbnail = images[0];
                        var fileId = wordpressService.UploadImage(
                            user: wordpress.Username,
                            pass: password,
                            baseUrl: wordpress.Url,
                            blogId: blogId,
                            imageUrl: imageThumbnail.Url,
                            maxHeight: 320,
                            maxWidth: 240);

                        if (fileId != "-1")
                        {
                            wordpressService.EditPostThumbnail(
                                user: wordpress.Username,
                                pass: password,
                                baseUrl: wordpress.Url,
                                blogId: blogId,
                                postId: postId,
                                imageId: fileId);
                        }
                    }
                }
            }

            //REFRESH CACHE
            RefreshCache(wordpress.Url);
            string postUrl = wordpressService.GetPostUrl(
                user: wordpress.Username,
                pass: password,
                baseUrl: wordpress.Url,
                blogId: blogId,
                postId: postId);
            if (postUrl.Length > 0) RefreshCache(postUrl);

        }

        private static void RefreshCache(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.GetAsync(url).GetAwaiter().GetResult();
                }
            }
            catch(Exception e)
            {

            }
        }

        private static int GetCountFromMask(string countMask)
        {
            int count = 0;
            if(countMask!=null)
            {
                if(countMask.Contains('-'))
                {
                    int from;
                    int to;

                    string[] ranges = countMask.Split('-');

                    if(Int32.TryParse(ranges[0], out from))
                    {
                        if(Int32.TryParse(ranges[1], out to))
                        {
                            var rand = new Random();
                            count = rand.Next(from,to+1);
                        }
                    }
                }
                else
                {
                    Int32.TryParse(countMask, out count);
                }
            }
            return count;
        }
    }
}
