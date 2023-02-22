using Newtonsoft.Json;
using System;

namespace DataOperation.Models
{
    public class Payer
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("payment")]
        public decimal Payment { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("account_number")]
        public long AccountNumber { get; set; }
    }
}
