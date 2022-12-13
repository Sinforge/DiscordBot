using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot
{
    public class Program
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _client.Log += Log;
            var token = "MTA1MjI0NDk5NTU4OTAyNTkyNQ.GlwkyP.gCC8yc30ZKI994P4IpEz4jnmxbTKOro0dX-EQE";
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            //Hook into the message received event, this is how we handle the hello world example
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
        private async Task CommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // a command execution was attempted
        }
    }
}