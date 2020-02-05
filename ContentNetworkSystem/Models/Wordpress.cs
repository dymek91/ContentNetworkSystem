
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentNetworkSystem.Models
{
    public class Wordpress : Content
    {
        public string XmlRPCUrl { get; set; }
        public int? TextGenerationCategoryId { get; set; }

        public override void PushContent()
        { 
            Console.WriteLine("Pushing wordpress content"); 
        }
    }
}
