using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot.Modules
{
    public class CommonCommands : ModuleBase<SocketCommandContext>
    {

        [MyPrecondition("Admin")]
        [Command("who-noob")]
        public async Task ping()
        {
            await ReplyAsync("HeqlinXNX");
        }



        [Command("userinfo")]
        public async Task GetUserInfo(SocketUser user = null)
        {
            var info = user ?? Context.Client.CurrentUser;
            await Context.Channel.SendMessageAsync($"{info.Username}#{info.Discriminator}");
        }


    }
}
