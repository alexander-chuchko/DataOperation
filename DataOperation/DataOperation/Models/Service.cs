using Newtonsoft.Json;
using System.Collections.Generic;

namespace DataOperation.Models
{
    public class Service
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("payers")]
        public List<Payer> Payers { get; set; }
        
        [JsonProperty("total")]
        public decimal Total { get; set; }
    }
}
