using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sentry;
using System.Reflection;

namespace Finbot.Discord
{
    public class FinbotDiscordBot
    {
        private readonly ILogger<FinbotDiscordBot> _logger;
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactionService;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _token;

        public FinbotDiscordBot(
            ILogger<FinbotDiscordBot> logger,
            DiscordSocketClient client,
            InteractionService interactionService,
            IConfiguration config,
            IServiceProvider provider)
        {
            _logger = logger;
            _client = client;
            _interactionService = interactionService;
            _serviceProvider = provider;
            _token = config.GetValue<string>("Discord:token") ?? "";
            _client.Log += Log;
        }

        public async Task Initialize() 
        {
            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();
            _client.SlashCommandExecuted += SlashCommandExecuted;
            _client.Ready += OnReady;
        }

        private async Task SlashCommandExecuted(SocketSlashCommand command)
        {
            if (command == null) return;

            var transaction = SentrySdk.StartTransaction(
                "command-execute",
                command.CommandName
            );
            
            transaction.SetExtra("guildId", command.GuildId.ToString());
            transaction.SetExtra("params", string.Join(";", command.Data.Options.Select(m => $"{m.Name}:{m.Value}")));

            transaction.User = new User() { Id = command.User.Id.ToString(), Username = command.User.Username };

            var ctx = new InteractionContext(_client, command);
            await _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);

            transaction.Finish();
        }

        private async Task OnReady()
        {
            await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider);
#if DEBUG
            await _interactionService.RegisterCommandsToGuildAsync(497583774045831185, true);
#else
            await _interactionService.RegisterCommandsGloballyAsync(true);
#endif
        }

        private Task Log(LogMessage msg)
        {
            var severity = msg.Severity switch
            {
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Info => LogLevel.Information,
                _ => LogLevel.Debug
            };

            _logger.Log(severity, msg.Message);
            if (msg.Exception != null)
            {
                _logger.Log(severity, msg.Exception.ToString());
                SentrySdk.CaptureException(msg.Exception);
            }

            return Task.CompletedTask;
        }
    }
}