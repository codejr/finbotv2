﻿using System.Text;

namespace Finbot.Models
{
    public class Portfolio
    {
        public int PortfolioId { get; set; }

        public ulong DiscordUserId { get; set; }

        public decimal CashBalance { get; set; }

        public List<Position> Positions { get; } = new List<Position>();

        public decimal MarketValue => this.Positions.Sum(p => p.MarketValue) + this.CashBalance;

        public decimal UnrealizedPnL => this.MarketValue - this.Positions.Sum(p => p.AveragePrice * p.Quantity);

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var position in this.Positions)
            {
                sb.Append($"{position.Symbol} - {position.Quantity:#} shares - Market Value: {position.MarketValue:C} - Unrealized PnL:{position.UnrealizedPnl:C}\r\n");
            }
            sb.Append($"Cash: {this.CashBalance:C}\r\n");

            sb.Append($"**Total Market Value: {this.MarketValue:C}**");

            return sb.ToString();
        }
    }
}
