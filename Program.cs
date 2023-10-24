using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

public class Program
{
    private DiscordSocketClient _client;
    private CommandService _commands;

    public static async Task Main(string[] args)
    {
        Program program = new Program();
        await program.MainAsync();
    }

    public async Task MainAsync()
    {
        _client = new DiscordSocketClient();
        _commands = new CommandService();

        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;
        _client.InteractionCreated += InteractionCreatedAsync;

        string token = "YOUR_BOT_TOKEN";
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await _commands.AddModuleAsync<CommandModule>(null);

        await Task.Delay(-1);
    }

    private Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log);
        return Task.CompletedTask;
    }

    private Task ReadyAsync()
    {
        Console.WriteLine("Bot is connected");
        return Task.CompletedTask;
    }

    private async Task InteractionCreatedAsync(SocketInteraction interaction)
    {
        if (interaction is SocketSlashCommand slashCommand)
        {
            var context = new SocketCommandContext(_client, slashCommand);

            if (_commands.Commands.Any(x => x.Name == slashCommand.Data.Name))
            {
                var result = await _commands.ExecuteAsync(context, 0, null);
                if (!result.IsSuccess)
                {
                    await context.Channel.SendMessageAsync($"Error: {result.ErrorReason}");
                }
            }
        }
    }
}

public class CommandModule : ModuleBase<SocketCommandContext>
{
    [SlashCommand("hello", "Say hello")]
    public async Task HelloCommand()
    {
        await ReplyAsync("Hello!");
    }
}
