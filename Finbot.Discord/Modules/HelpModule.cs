using Discord;
using Discord.Interactions;

namespace Finbot.Discord.Modules
{
    public class HelpModule : InteractionModuleBase
    {
        private readonly InteractionService _interactionService;

        public HelpModule(InteractionService interactionService)
        {
            _interactionService = interactionService;
        }

        [SlashCommand("help", "Shows help on how to use the bot")]
        public async Task HelpCommand()
        {
            await DeferAsync(ephemeral: true);

            var commands = _interactionService.SlashCommands;

            var embedBuilder = new EmbedBuilder().WithTitle("Help").WithColor(Color.Green);

            foreach (var command in commands)
            {
                var embedFieldText = command.Description ?? "No description available\n";

                embedBuilder.AddField(command.Name, embedFieldText);
            }

            await FollowupAsync("Here's a list of commands", embed: embedBuilder.Build(), ephemeral:  true);
        }
    }
}
