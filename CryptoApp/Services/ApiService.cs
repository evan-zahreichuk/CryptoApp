using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CryptoApp.Models;
using Newtonsoft.Json;
namespace CryptoApp.Services
{
    public class ApiService
    {
        private HttpClient _client;
        public ApiService()
        {
            _client = new HttpClient();
        }
        public async Task<List<Currency>> GetTopCurrenciesAsync()
        {
            string url = "https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&order=market_cap_desc&per_page=10&page=1&sparkline=false";
            var response = await _client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<CoinMarketDto>>(json);
                return list.Select(c => new Currency
                {
                    Id = c.id,
                    Name = c.name,
                    Symbol = c.symbol.ToUpper(),
                    PriceUsd = c.current_price.ToString(),
                    VolumeUsd = c.total_volume.ToString(),
                    ChangePercent24Hr = c.price_change_percentage_24h.ToString()
                }).ToList();
            }
            return new List<Currency>();
        }
        public async Task<Currency> GetCurrencyDetailAsync(string id)
        {
            string url = $"https://api.coingecko.com/api/v3/coins/{id}?localization=false&tickers=false&market_data=true&community_data=false&developer_data=false&sparkline=false";
            var response = await _client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var detail = JsonConvert.DeserializeObject<CoinDetailDto>(json);
                return new Currency
                {
                    Id = detail.id,
                    Name = detail.name,
                    Symbol = detail.symbol.ToUpper(),
                    PriceUsd = detail.market_data.current_price["usd"].ToString(),
                    VolumeUsd = detail.market_data.total_volume["usd"].ToString(),
                    ChangePercent24Hr = detail.market_data.price_change_percentage_24h.ToString()
                };
            }
            return null;
        }
        private class CoinMarketDto
        {
            public string id { get; set; }
            public string symbol { get; set; }
            public string name { get; set; }
            public decimal current_price { get; set; }
            public decimal total_volume { get; set; }
            public decimal price_change_percentage_24h { get; set; }
        }
        private class CoinDetailDto
        {
            public string id { get; set; }
            public string symbol { get; set; }
            public string name { get; set; }
            public MarketData market_data { get; set; }
        }
        private class MarketData
        {
            public Dictionary<string, decimal> current_price { get; set; }
            public Dictionary<string, decimal> total_volume { get; set; }
            public decimal price_change_percentage_24h { get; set; }
        }
    }
}
