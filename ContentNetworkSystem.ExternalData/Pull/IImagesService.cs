using System;
using System.Collections.Generic;
using System.Text;
using ContentNetworkSystem.Models;
using ContentNetworkSystem.Models.GoogleSearchCache;
using System.Threading.Tasks;

namespace ContentNetworkSystem.Pull
{
    public interface IImagesService
    {
        Task<List<ImagesResult>> SearchImages_Api(Niche niche, int imagesToGet);
    }
}
