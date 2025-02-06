namespace CryptoApp.Models
{
    public class Currency
    {
        public string Id { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string PriceUsd { get; set; }
        public string VolumeUsd { get; set; }
        public string ChangePercent24Hr { get; set; }
    }
}
