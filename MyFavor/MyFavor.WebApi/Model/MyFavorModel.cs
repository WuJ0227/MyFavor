using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFavor.Model
{
    public class MyFavorModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        [JsonProperty("full_name")]
        public string FullName { get; set; }
        public string Description { get; set; }
        [JsonProperty("url")]
        public string HtmlUrl { get; set; }

    }
}
