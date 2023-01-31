using Discord.Commands;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordBot.Modules
{
    public class Command : ModuleBase<SocketCommandContext>
    {

        [MyPrecondition("Admin")]
        [Command("who-noob")]
        public async Task ping()
        {
            await ReplyAsync("HeqlinXNX");
        }

        [Command("join", RunMode = Discord.Commands.RunMode.Async)]
        public async Task Join()
        {
            var channel = (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null)
            {
                await ReplyAsync("User must be in voice channel");
            }

            var audioClient = await channel.ConnectAsync();
        }

        [Command("userinfo")]
        public async Task GetUserInfo(SocketUser user = null)
        {
            var info = user ?? Context.Client.CurrentUser;
            await Context.Channel.SendMessageAsync($"{info.Username}#{info.Discriminator}");
        }

        
    }

    public class InteractionCommands : InteractionModuleBase
    {
        [SlashCommand("echo", "Echo an input")]
        public async Task echo(string input)
        {
            await RespondAsync(input);
        }
    }

    [Discord.Commands.Group("arith")]
    public class ArithmeticCommand : ModuleBase<SocketCommandContext>
    {
        [Command("sum")]
        [Discord.Commands.Summary("Enter two nums to sum")]
        public async Task sum(double num1, double num2)
        {
            await ReplyAsync($"{num1} + {num2} = {num1 + num2}");
        }

        [Command("square")]
        [Discord.Commands.Summary("Square number")]
        public async Task square(double num)
        {
            await Context.Channel.SendMessageAsync($"{num}^2 = {num * num}");
        }
    }


}