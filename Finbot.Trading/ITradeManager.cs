namespace Finbot.Trading
{
    public interface ITradeManager
    {
        Task<TradeExecutionResult> Buy(ulong id, string symbol, int count);
        Task<TradeExecutionResult> Sell(ulong id, string symbol, int count);
    }
}