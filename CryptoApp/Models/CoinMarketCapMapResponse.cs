using System.Collections.Generic;
using Newtonsoft.Json;

namespace CryptoApp.Models
{
    public class CoinMarketCapMapResponse
    {
        [JsonProperty("data")]
        public List<MapCoinData> Data { get; set; }
    }
}
