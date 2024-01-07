using Finbot.Discord;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Finbot.Service
{
    public class FinbotService : BackgroundService
    {
        private ILogger<FinbotService> _logger;
        private readonly FinbotDiscordBot _bot;

        public FinbotService(ILogger<FinbotService> logger, FinbotDiscordBot bot)
        {
            this._logger = logger;
            this._bot = bot;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service starting up");
            await _bot.Initialize();
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1_000, stoppingToken);
            }
        }
    }
}
