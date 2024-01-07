using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;

namespace Finbot.MarketData
{
    public class MarketDataClient : IMarketDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MarketDataClient> _logger;

        public MarketDataClient(HttpClient httpClient, ILogger<MarketDataClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<decimal?> GetPriceAsync(string ticker) 
        {
            try
            {
                var resp = await _httpClient.GetAsync(ticker);
                var json = await resp.Content.ReadFromJsonAsync<MarketDataResponse>();
                return json?.Price;
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Unable to get market data for ticker:{ticker}. Exception: {ex}");
            }

            return null;
        }
    }
}