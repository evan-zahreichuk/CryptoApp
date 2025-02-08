using Newtonsoft.Json;

namespace CryptoApp.Models
{
    public class CoinQuote
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("quote")]
        public QuoteData Quote { get; set; }
    }
}
