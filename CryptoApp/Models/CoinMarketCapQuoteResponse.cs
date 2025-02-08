using System.Collections.Generic;
using Newtonsoft.Json;

namespace CryptoApp.Models
{
    public class CoinMarketCapQuoteResponse
    {
        [JsonProperty("data")]
        public Dictionary<string, CoinQuote> Data { get; set; }
    }
}
