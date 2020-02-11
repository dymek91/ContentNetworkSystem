using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentNetworkSystem.Models;

namespace ContentNetworkSystem.Models.ViewModels
{
    public class AvailableContent
    {
        private List<Content> _contents;
        public AvailableContent()
        {
            _contents = new List<Content>()
            {
                new Wordpress() { TypeName = "Wordpress", BlogId = 1 }
            };
        }
        public List<Content> GetContents()
        {
            return _contents;
        }
        public Content GetByTypeName(string typeName)
        {
            Content content = null;

            content =  _contents.Find(e => e.TypeName == typeName);

            return content;
        }
        //public List<Content> GetContents = new List<Content>()
        //{
        //    new Wordpress() { TypeName = "Wordpress" }
        //};
    }
}
