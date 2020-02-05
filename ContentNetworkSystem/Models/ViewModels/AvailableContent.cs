using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentNetworkSystem.Models;

namespace ContentNetworkSystem.Models.ViewModels
{
    public class AvailableContent
    {
        public List<Content> GetContents = new List<Content>()
        {
            new Wordpress() { TypeName = "Wordpress" }
        };
    }
}
