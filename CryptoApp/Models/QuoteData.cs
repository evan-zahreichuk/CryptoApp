using Newtonsoft.Json;

namespace CryptoApp.Models
{
    public class QuoteData
    {
        [JsonProperty("USD")]
        public UsdData USD { get; set; }
    }
}
