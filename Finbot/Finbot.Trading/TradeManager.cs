using Finbot.Data;
using Finbot.MarketData;
using Finbot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Finbot.Trading
{
    public class TradeManager : ITradeManager
    {
        private readonly FinbotDataContext _db;
        private readonly IMarketDataClient _marketDataService;
        private readonly IPortfolioManager _portfolioManager;
        private readonly ILogger<TradeManager> _logger;

        public TradeManager(
            FinbotDataContext db,
            IMarketDataClient marketDataService,
            IPortfolioManager portfolioManager,
            ILogger<TradeManager> logger)
        {
            this._db = db;
            this._marketDataService = marketDataService;
            this._portfolioManager = portfolioManager;
            this._logger = logger;
        }

        public async Task<TradeExecutionResult> Buy(ulong id, string symbol, int count)
        {
            var portfolio = await _portfolioManager.GetPortfolioAsync(id);

            var price = await _marketDataService.GetPriceAsync(symbol);

            if (price == null)
            {
                return new TradeExecutionResult(false, $"Unable to price ticker {symbol}", null);
            }

            using var transaction = _db.Database.BeginTransaction();

            try
            {
                var trade = new Trade()
                {
                    PortfolioId = portfolio.PortfolioId,
                    ExecutionPrice = price ?? 0,
                    Symbol = symbol,
                    ExecutionTime = DateTimeOffset.UtcNow,
                    Quantity = count,
                    Side = TradeSide.Buy,
                };

                var totalCost = trade.ExecutionPrice * trade.Quantity;

                if (totalCost > portfolio.CashBalance)
                {
                    return new TradeExecutionResult(false, $"Unable to purchase {count} shares of {symbol}.; Balance too low.", trade);
                }

                portfolio.CashBalance -= totalCost;

                _db.Attach(portfolio);

                var position = await _db.Positions
                    .FirstOrDefaultAsync(m => m.PortfolioId == portfolio.PortfolioId && m.Symbol == symbol);

                if (position == null)
                {
                    position = new Position()
                    {
                        PortfolioId = portfolio.PortfolioId,
                        Symbol = symbol,
                        LatestPrice = trade.ExecutionPrice,
                        AveragePrice = trade.ExecutionPrice,
                        Quantity = 0
                    };

                    _db.Positions.Attach(position);
                }

                position.AveragePrice = (await _db.Trades
                    .Where(m => m.PortfolioId == portfolio.PortfolioId && m.Symbol == symbol)
                    .Select(m => m.ExecutionPrice)
                    .ToListAsync())
                    .Concat(new[] { trade.ExecutionPrice })
                    .Average();

                position.Quantity += trade.Quantity;
                position.LatestPrice = trade.ExecutionPrice;
                await _db.Trades.AddAsync(trade);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return new TradeExecutionResult(true, null, trade);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving trade. {ex}");

                return new TradeExecutionResult(false, "Unknown error", null);
            }
        }

        public async Task<TradeExecutionResult> Sell(ulong id, string symbol, int count)
        {
            var portfolio = await _portfolioManager.GetPortfolioAsync(id);

            var position = await _db.Positions
                    .FirstOrDefaultAsync(m => m.PortfolioId == portfolio.PortfolioId && m.Symbol == symbol);

            if (position == null || position.Quantity < count)
            {
                return new TradeExecutionResult(false, $"You dont own enough shares of {symbol} to sell", null);
            }

            var price = await _marketDataService.GetPriceAsync(symbol);

            if (price == null)
            {
                return new TradeExecutionResult(false, $"Unable to price ticker {symbol}", null);
            }

            using var transaction = _db.Database.BeginTransaction();

            try
            {
                var trade = new Trade()
                {
                    PortfolioId = portfolio.PortfolioId,
                    ExecutionPrice = price ?? 0,
                    Symbol = symbol,
                    ExecutionTime = DateTimeOffset.UtcNow,
                    Quantity = count,
                    Side = TradeSide.Sell,
                };

                var totalCost = trade.ExecutionPrice * trade.Quantity;

                portfolio.CashBalance += totalCost;

                _db.Attach(portfolio);

                await _db.Trades.AddAsync(trade);
                position.Quantity -= trade.Quantity;
                position.LatestPrice = trade.ExecutionPrice;

                if (position.Quantity <= 0)
                {
                    _db.Remove(position);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return new TradeExecutionResult(true, null, trade);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving trade. {ex}");
                await transaction.RollbackAsync();
                return new TradeExecutionResult(false, "Unknown error", null);
            }
        }

    }
}
