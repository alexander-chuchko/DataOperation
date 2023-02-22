using Newtonsoft.Json;
using System.Collections.Generic;

namespace DataOperation.Models
{
    public class Root
    {
        public Root()
        {
            Services = new List<Service>();
        }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("services")]
        public List<Service> Services { get; set; }

        [JsonProperty("total")]
        public decimal Total { get; set; }
    }
}
