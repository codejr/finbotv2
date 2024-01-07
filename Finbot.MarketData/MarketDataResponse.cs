namespace Finbot.MarketData
{
    internal record MarketDataResponse
    {
        public decimal? Price { get; init; }
    }
}