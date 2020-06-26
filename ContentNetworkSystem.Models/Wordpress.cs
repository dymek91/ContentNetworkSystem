namespace ContentNetworkSystem.Models
{
    public class Wordpress : Content
    { 
        public int? TextGenerationCategoryId { get; set; }
        public int? BlogId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool? AddThumbnail { get; set; } = false;
        public string ImagesCount { get; set; }
        public string VideosCount { get; set; }
        public string AuthorityLinksCount { get; set; } 

        //public override void EncryptPassword(IServiceProvider serviceProvider)
        //{
        //    throw new NotImplementedException();
        //}
        //public override async  Task PushContent(IServiceProvider serviceProvider, IHttpClientFactory clientFactory)
        //{
        //    throw new NotImplementedException();
        //}

        //    public override void EncryptPassword(IServiceProvider serviceProvider);
        //    {
        //        EncryptionService encryptionService = serviceProvider.GetService<EncryptionService>();
        //    Password = encryptionService.EncryptString(Password);
        //    }

        //public override async  Task PushContent(IServiceProvider serviceProvider, IHttpClientFactory clientFactory)
        //    { 
        //        Console.WriteLine("Pushing wordpress content");

        //        int blogId = 1;
        //        if (BlogId.HasValue) { blogId = BlogId.Value; }

        //        var wordpressService =  serviceProvider.GetService<WordpressService>();
        //        var textGenerationService = serviceProvider.GetService<TextGenerationService>();
        //        var encryptionService = serviceProvider.GetService<EncryptionService>();
        //        var googleImagesService = serviceProvider.GetService<GoogleImagesService>();
        //        var randomContentService = serviceProvider.GetService<RandomContentService>();

        //        TextJSON textJSON = await textGenerationService.GetText(TextGenerationCategoryId.Value);
        //        string postTitle = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(textJSON.SuggestedTitle));  
        //        string postContent= System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(textJSON.Text));
        //        string password = encryptionService.DecryptString(Password);

        //        //ADD AUTHORITY LINKS
        //        int authorityLinksToAdd = GetCountFromMask(AuthorityLinksCount);
        //        if (authorityLinksToAdd > 0)
        //        {
        //            postContent = randomContentService.InsertLinksToText(postContent, authorityLinksToAdd);
        //        } 

        //        if (Project.Niche == null)
        //        {
        //            wordpressService.PushPost(
        //                user: Username,
        //                pass: password,
        //                baseUrl: Url,
        //                blogId: blogId,
        //                postTitle: postTitle,
        //                postContent: postContent);
        //            return;
        //        } 

        //        //ADD VIDEO
        //        int videosToAdd = GetCountFromMask(VideosCount);
        //        if (videosToAdd>0)
        //        {
        //            postContent = await randomContentService.InsertVideosToText(Project.Niche, postContent, videosToAdd);
        //        }

        //        //ADD IMAGES 
        //        int imagesToAdd = GetCountFromMask(ImagesCount); 

        //        if (imagesToAdd > 0)
        //        {
        //            postContent = await randomContentService.InsertImagesToText(Project.Niche, postContent, imagesToAdd);
        //        }

        //        //SEND POST
        //        var postId = wordpressService.PushPost(
        //            user: Username,
        //            pass: password,
        //            baseUrl: Url,
        //            blogId:blogId,
        //            postTitle:postTitle,
        //            postContent: postContent);

        //        //ADD THUMBNAIL
        //        if(AddThumbnail.HasValue)
        //        {
        //            if(AddThumbnail.Value)
        //            {
        //                var images = await googleImagesService.SearchImages_Api(Project.Niche, 1);
        //                if (images.Count>0)
        //                {
        //                    var imageThumbnail = images[0];
        //                    var fileId = wordpressService.UploadImage(
        //                        user: Username,
        //                        pass: password,
        //                        baseUrl: Url,
        //                        blogId: blogId,
        //                        imageUrl: imageThumbnail.Url,
        //                        maxHeight: 320,
        //                        maxWidth: 240);

        //                    if (fileId != "-1")
        //                    {
        //                        wordpressService.EditPostThumbnail(
        //                            user: Username,
        //                            pass: password,
        //                            baseUrl: Url,
        //                            blogId: blogId,
        //                            postId: postId,
        //                            imageId: fileId);
        //                    }
        //                }
        //            }
        //        }

        //    }

        //    private int GetCountFromMask(string countMask)
        //    {
        //        int count = 0;
        //        if(countMask!=null)
        //        {
        //            if(countMask.Contains('-'))
        //            {
        //                int from;
        //                int to;

        //                string[] ranges = countMask.Split('-');

        //                if(Int32.TryParse(ranges[0], out from))
        //                {
        //                    if(Int32.TryParse(ranges[1], out to))
        //                    {
        //                        var rand = new Random();
        //                        count = rand.Next(from,to+1);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                Int32.TryParse(countMask, out count);
        //            }
        //        }
        //        return count;
        //    }
    }
}
