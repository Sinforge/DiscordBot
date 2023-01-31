using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot.Modules
{
    public class MyPreconditionAttribute: PreconditionAttribute
    {
        private readonly string _role;

        public MyPreconditionAttribute(string role)
        {
            _role = role;
        }
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.User is SocketGuildUser user)
            {
                if (user.Roles.Any(r => r.Name == _role))
                {
                    return Task.FromResult(PreconditionResult.FromSuccess());
                }
                else
                {
                    context.Channel.SendMessageAsync($"You must have a role named {_role} to use this command");
                    return Task.FromResult(
                        PreconditionResult.FromError($"You must have a role named {_role} to use this command"));
                }
            }
            else
            {
                context.Channel.SendMessageAsync("You must be in a guild to run this command.");
                return Task.FromResult(PreconditionResult.FromError("You must be in a guild to run this command."));
            }
            throw new NotImplementedException();
        }
    }
}
