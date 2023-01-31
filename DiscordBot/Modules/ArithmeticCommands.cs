using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot.Modules
{
    [Discord.Commands.Group("arith")]
    public class ArithmeticCommands : ModuleBase<SocketCommandContext>
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
