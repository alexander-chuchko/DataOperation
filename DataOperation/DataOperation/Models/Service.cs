using Newtonsoft.Json;
using System.Collections.Generic;

namespace DataOperation.Models
{
    public class Service
    {
        public Service()
        {
            Payers = new List<Payer>();
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("payers")]
        public List<Payer> Payers { get; set; }
        
        [JsonProperty("total")]
        public decimal Total { get; set; }
    }
}
