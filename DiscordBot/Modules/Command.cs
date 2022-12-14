using Discord.Commands;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class Command : ModuleBase<SocketCommandContext>
    {

        [Command("who-noob")]
        public async Task ping()
        {
            await ReplyAsync("HeqlinXNX");
        }
    }
}