using Newtonsoft.Json;

namespace CryptoApp.Models
{
    public class UsdData
    {
        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("volume_24h")]
        public decimal Volume24H { get; set; }

        [JsonProperty("percent_change_24h")]
        public decimal PercentChange24H { get; set; }
    }
}
