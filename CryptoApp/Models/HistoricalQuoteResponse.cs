using System.Collections.Generic;
using Newtonsoft.Json;

namespace CryptoApp.Models
{
    public class HistoricalQuoteResponse
    {
        [JsonProperty("data")]
        public List<HistoricalQuote> Data { get; set; }
    }
}
