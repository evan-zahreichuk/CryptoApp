using System;
using Newtonsoft.Json;

namespace CryptoApp.Models
{
    public class HistoricalQuote
    {
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }
    }
}
