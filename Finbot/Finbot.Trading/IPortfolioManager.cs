using Finbot.Models;

namespace Finbot.Trading
{
    public interface IPortfolioManager
    {
        Task<Portfolio> GetPortfolioAsync(ulong userId);

        Task<Position?> GetPositionAsync(ulong userId, string symbol);

        Task SetBalanceAsync(ulong userId, decimal balance);
    }
}