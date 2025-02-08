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

        public async Task<List<Currency>> GetTopCurrenciesAsync(int start, int limit)
        {
            string url = $"{_baseUrl}cryptocurrency/listings/latest?start={start}&limit={limit}&convert=USD";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-CMC_PRO_API_KEY", _apiKey);
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<CoinMarketCapListingsResponse>(json);
                return result.Data.Select(item => new Currency
                {
                    Id = item.Id.ToString(),
                    Name = item.Name,
                    Symbol = item.Symbol,
                    PriceUsd = item.Quote.USD.Price.ToString(),
                    VolumeUsd = item.Quote.USD.Volume24H.ToString(),
                    ChangePercent24Hr = item.Quote.USD.PercentChange24H.ToString()
                }).ToList();
            }
            else
            {
                throw new Exception($"Error retrieving top currencies: {response.ReasonPhrase}");
            }
        }

        public async Task<Currency> GetCurrencyDetailAsync(string id)
        {
            string url = $"{_baseUrl}cryptocurrency/quotes/latest?id={id}&convert=USD";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-CMC_PRO_API_KEY", _apiKey);
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<CoinMarketCapQuoteResponse>(json);
                if (result.Data.TryGetValue(id, out CoinQuote coinQuote))
                {
                    return new Currency
                    {
                        Id = id,
                        Name = coinQuote.Name,
                        Symbol = coinQuote.Symbol,
                        PriceUsd = coinQuote.Quote.USD.Price.ToString(),
                        VolumeUsd = coinQuote.Quote.USD.Volume24H.ToString(),
                        ChangePercent24Hr = coinQuote.Quote.USD.PercentChange24H.ToString()
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

        public async Task<List<Currency>> SearchCurrenciesAsync(string searchQuery)
        {
            string mapUrl = $"{_baseUrl}cryptocurrency/map?limit=5000";
            var mapRequest = new HttpRequestMessage(HttpMethod.Get, mapUrl);
            mapRequest.Headers.Add("X-CMC_PRO_API_KEY", _apiKey);
            var mapResponse = await _client.SendAsync(mapRequest);
            if (!mapResponse.IsSuccessStatusCode)
                throw new Exception("Error retrieving cryptocurrency map: " + mapResponse.ReasonPhrase);
            var mapJson = await mapResponse.Content.ReadAsStringAsync();
            var mapResult = JsonConvert.DeserializeObject<CoinMarketCapMapResponse>(mapJson);
            var filteredCoins = mapResult.Data.Where(c => c.Name.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                                          c.Symbol.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                                          c.Slug.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                                              .ToList();
            if (filteredCoins.Count == 0)
                return new List<Currency>();
            var limitedCoins = filteredCoins.Take(1000).ToList();
            string ids = string.Join(",", limitedCoins.Select(c => c.Id));
            string quotesUrl = $"{_baseUrl}cryptocurrency/quotes/latest?id={ids}&convert=USD";
            var quotesRequest = new HttpRequestMessage(HttpMethod.Get, quotesUrl);
            quotesRequest.Headers.Add("X-CMC_PRO_API_KEY", _apiKey);
            var quotesResponse = await _client.SendAsync(quotesRequest);
            if (!quotesResponse.IsSuccessStatusCode)
                throw new Exception("Error retrieving currency quotes: " + quotesResponse.ReasonPhrase);
            var quotesJson = await quotesResponse.Content.ReadAsStringAsync();
            var quotesResult = JsonConvert.DeserializeObject<CoinMarketCapQuoteResponse>(quotesJson);
            return limitedCoins.Where(c => quotesResult.Data.ContainsKey(c.Id.ToString()))
                               .Select(c =>
                               {
                                   var coinQuote = quotesResult.Data[c.Id.ToString()];
                                   return new Currency
                                   {
                                       Id = c.Id.ToString(),
                                       Name = c.Name,
                                       Symbol = c.Symbol,
                                       PriceUsd = coinQuote.Quote.USD.Price.ToString(),
                                       VolumeUsd = coinQuote.Quote.USD.Volume24H.ToString(),
                                       ChangePercent24Hr = coinQuote.Quote.USD.PercentChange24H.ToString()
                                   };
                               }).ToList();
        }
    }
}
