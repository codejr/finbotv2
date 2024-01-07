using Discord;
using Discord.Interactions;
using Finbot.Trading;

namespace Finbot.Discord.Modules
{
    public class PortfolioModule : InteractionModuleBase
    {
        private readonly IPortfolioManager _portfolioManager;
        private readonly ITradeManager _tradeManager;

        public PortfolioModule(IPortfolioManager portfolioManager, ITradeManager tradeManager) 
        {
            this._portfolioManager = portfolioManager;
            this._tradeManager = tradeManager;
        }

        [SlashCommand("balance", "Set a user's balance. Requires bot owner permission"), RequireOwner]
        public async Task SetBalance(IUser user, decimal balance)
        {
            await DeferAsync();

            await _portfolioManager.SetBalanceAsync(user.Id, balance);

            await FollowupAsync($"Set {user.Mention}'s balance to {balance:c}");
        }

        [SlashCommand("buy", "buy x shares of a stock")]
        public async Task Buy(string symbol, int count) 
        {
            await DeferAsync();

            if (count <= 0)
            {
                await RespondAsync("You must buy at least one share", ephemeral: true);
                return;
            }

            var realSymbol = symbol.Trim().ToUpper();

            var result = await _tradeManager.Buy(Context.User.Id, realSymbol, count);

            if (result.Success)
            {
                await FollowupAsync($"Successfully purchased {count} shares of {realSymbol} at {result.Trade?.ExecutionPrice:c}");
            }
            else
            {
                await FollowupAsync($"Error making purchase: {result.Message}");
            }
        }

        [SlashCommand("sell", "sells x shares of a stock from your portfolio")]
        public async Task Sell(string symbol, int count)
        {
            await DeferAsync();

            if (count <= 0) 
            {
                await RespondAsync("You must sell at least one share", ephemeral:  true);
                return;
            }

            var realSymbol = symbol.Trim().ToUpper();

            var result = await _tradeManager.Sell(Context.User.Id, realSymbol, count);

            if (result.Success)
            {
                await FollowupAsync($"Successfully sold {count} shares of {realSymbol} at {result.Trade?.ExecutionPrice:c}");
            }
            else
            {
                await FollowupAsync($"Error making sale: {result.Message}");
            }
        }

        [SlashCommand("portfolio", "see your own portfolio or someone else's")]
        public async Task ViewPortfolio(IUser? user = null)
        {
            await DeferAsync();

            var userId = user?.Id ?? Context.User.Id;

            var portfolio = await _portfolioManager.GetPortfolioAsync(userId);

            await FollowupAsync(portfolio.ToString());
        }
    }
}
