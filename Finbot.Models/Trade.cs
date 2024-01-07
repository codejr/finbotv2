namespace Finbot.Models
{
    public enum TradeSide
    {
        Buy,
        Sell
    }

    public record Trade()
    {
        public int TradeId { get; set; }

        public int PortfolioId { get; set; }

        public required string Symbol { get; set; }

        public  TradeSide Side { get; set; }

        public decimal Quantity { get; set; }

        public decimal ExecutionPrice { get; set; }

        public DateTimeOffset ExecutionTime { get; set; }
    }
}
