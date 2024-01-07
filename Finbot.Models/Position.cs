namespace Finbot.Models
{
    public class Position
    {
        public int PositionId { get; set; }

        public required string Symbol { get; set; }

        public decimal Quantity { get; set; }

        public decimal AveragePrice { get; set; }

        public decimal LatestPrice { get; set; }

        public int PortfolioId { get; set; }

        public Portfolio? Portfolio { get; set; }

        public decimal UnrealizedPnl => this.Quantity * (this.LatestPrice - this.AveragePrice);

        public decimal MarketValue => this.LatestPrice * this.Quantity;
    }

}