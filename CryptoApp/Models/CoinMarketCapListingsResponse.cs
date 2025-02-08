using System.Collections.Generic;
using Newtonsoft.Json;

namespace CryptoApp.Models
{
    public class CoinMarketCapListingsResponse
    {
        [JsonProperty("data")]
        public List<CoinData> Data { get; set; }
    }
}
