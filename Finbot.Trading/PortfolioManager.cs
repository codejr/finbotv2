using Finbot.Data;
using Finbot.MarketData;
using Finbot.Models;
using Microsoft.EntityFrameworkCore;

namespace Finbot.Trading
{
    public class PortfolioManager : IPortfolioManager
    {
        private readonly FinbotDataContext _db;
        private readonly IMarketDataClient _marketDataClient;

        public PortfolioManager(FinbotDataContext db, IMarketDataClient marketDataClient)
        {
            _db = db;
            _marketDataClient = marketDataClient;
        }

        private Portfolio CreateDefault(ulong userId)
        {
            return new Portfolio()
            {
                CashBalance = 100_000m,
                DiscordUserId = userId
            };
        }

        public async Task<Portfolio> GetPortfolioAsync(ulong userId)
        {
            var portfolio = await _db.Portfolios.Include(m => m.Positions).FirstOrDefaultAsync(m => m.DiscordUserId == userId);

            if (portfolio == null)
            {
                portfolio = CreateDefault(userId);
                _db.Portfolios.Add(portfolio);
                await _db.SaveChangesAsync();
            }
            else
            {
                foreach(var position in portfolio.Positions)
                {
                    var price = await _marketDataClient.GetPriceAsync(position.Symbol);
                    if (price != null)
                    {
                        position.LatestPrice = price.Value;
                    }
                }
            }

            return portfolio;
        }

        public async Task<Position?> GetPositionAsync(ulong userId, string symbol)
        {
            var position = await _db.Positions.Include(m => m.Portfolio)
                .FirstOrDefaultAsync(m => m.Portfolio.DiscordUserId == userId && m.Symbol == symbol);

            return position;
        }

        public async Task SetBalanceAsync(ulong userId, decimal balance)
        {
            var portfolio = await GetPortfolioAsync(userId);

            portfolio.CashBalance = balance;

            await _db.SaveChangesAsync();
        }
    }
}