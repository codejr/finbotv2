using Finbot.Models;
namespace Finbot.Trading
{
    public record TradeExecutionResult(bool Success, string? Message, Trade? Trade);
}
