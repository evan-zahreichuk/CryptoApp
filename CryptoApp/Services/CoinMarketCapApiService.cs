using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CryptoApp.Models;
using Newtonsoft.Json;

namespace CryptoApp.Services
{
    public class CoinMarketCapApiService
    {
        private readonly HttpClient _client;
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://pro-api.coinmarketcap.com/v1/";

        public CoinMarketCapApiService(string apiKey)
        {
            _client = new HttpClient();
            _apiKey = apiKey;
        }

        public async Task<List<Currency>> GetTopCurrenciesAsync(int limit = 10)
        {
            string url = $"{_baseUrl}cryptocurrency/listings/latest?limit={limit}&convert=USD";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-CMC_PRO_API_KEY", _apiKey);

            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<CoinMarketCapListingsResponse>(json);
                return result.data.Select(item => new Currency
                {
                    Id = item.id.ToString(),
                    Name = item.name,
                    Symbol = item.symbol,
                    PriceUsd = item.quote.USD.price.ToString(),
                    VolumeUsd = item.quote.USD.volume_24h.ToString(),
                    ChangePercent24Hr = item.quote.USD.percent_change_24h.ToString()
                }).ToList();
            }
            else
            {
                throw new Exception($"Error retrieving top currencies: {response.ReasonPhrase}");
            }
        }

        public async Task<Currency> GetCurrencyDetailAsync(string id)
        {
            // Для детальної інформації використовується endpoint quotes/latest
            string url = $"{_baseUrl}cryptocurrency/quotes/latest?id={id}&convert=USD";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-CMC_PRO_API_KEY", _apiKey);

            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<CoinMarketCapQuoteResponse>(json);
                if (result.data.TryGetValue(id, out CoinQuote coinQuote))
                {
                    return new Currency
                    {
                        Id = id,
                        Name = coinQuote.name,
                        Symbol = coinQuote.symbol,
                        PriceUsd = coinQuote.quote.USD.price.ToString(),
                        VolumeUsd = coinQuote.quote.USD.volume_24h.ToString(),
                        ChangePercent24Hr = coinQuote.quote.USD.percent_change_24h.ToString()
                    };
                }
                else
                {
                    throw new Exception("Currency detail not found in the response.");
                }
            }
            else
            {
                throw new Exception($"Error retrieving currency detail: {response.ReasonPhrase}");
            }
        }

        private class CoinMarketCapListingsResponse
        {
            public List<CoinData> data { get; set; }
        }

        private class CoinData
        {
            public int id { get; set; }
            public string name { get; set; }
            public string symbol { get; set; }
            public QuoteData quote { get; set; }
        }

        private class QuoteData
        {
            [JsonProperty("USD")]
            public UsdData USD { get; set; }
        }

        private class UsdData
        {
            public decimal price { get; set; }
            public decimal volume_24h { get; set; }
            public decimal percent_change_24h { get; set; }
        }

        private class CoinMarketCapQuoteResponse
        {
            public Dictionary<string, CoinQuote> data { get; set; }
        }

        private class CoinQuote
        {
            public int id { get; set; }
            public string name { get; set; }
            public string symbol { get; set; }
            public QuoteData quote { get; set; }
        }
    }
}
