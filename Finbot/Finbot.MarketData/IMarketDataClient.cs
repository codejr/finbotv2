namespace Finbot.MarketData
{
    public interface IMarketDataClient
    {
        public Task<decimal?> GetPriceAsync(string ticker);
    }
}