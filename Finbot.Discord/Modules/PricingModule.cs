using Discord.Interactions;
using Finbot.MarketData;

namespace Finbot.Discord.Modules
{
    public class PricingModule : InteractionModuleBase
    {
        private readonly IMarketDataClient mktDataClient;

        public PricingModule(IMarketDataClient mktDataClient)
        {
            this.mktDataClient = mktDataClient;
        }

        [SlashCommand("price", "Gets the price of a given stock")]
        public async Task GetPrice(string ticker, int shares = 1)
        {
            await DeferAsync(ephemeral: true);

            var realTicker = ticker.Trim().ToUpper();
            var price = await mktDataClient.GetPriceAsync(realTicker);

            var response = price == null 
                ? $"Cannot find price for {realTicker}" 
                : $"Price for *{shares}* shares of **{realTicker}**: *{price * shares:c}*";

            await FollowupAsync(response, ephemeral: true);
        }
    }
}
