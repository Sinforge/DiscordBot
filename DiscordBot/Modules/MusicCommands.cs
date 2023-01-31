using Discord.Commands;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Services;

namespace DiscordBot.Modules
{
    public class MusicCommands : ModuleBase<SocketCommandContext>
    {
        private readonly MusicService _musicService;

        public MusicCommands(MusicService musicService)
        {
            _musicService = musicService;
        }
        [Command("join")]
        public async Task Join()
        {
            var channel = (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null)
            {
                await ReplyAsync("User must be in voice channel");
                return;
            }

            await _musicService.ConnectAsync(channel, Context.Channel as ITextChannel);
        }
        [Command("leave")]
        public async Task Leave()
        {
            var channel = (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null)
            {
                await ReplyAsync("Bot not in channel");
                return;
            }

            await _musicService.LeaveAsync(channel);
        }

        [Command("play")]
        public async Task Play([Remainder] string query)
        {
            await ReplyAsync(await _musicService.PlayAsync(query, Context.Guild.Id));
        }
        [Command("stop")]
        public async Task Stop()
            => await ReplyAsync(await _musicService.StopAsync(Context.Guild.Id));

        [Command("skip")]
        public async Task Skip()
            => await ReplyAsync(await _musicService.SkipAsync(Context.Guild.Id));

        [Command("volume")]
        public async Task Volume(int vol)
            => await ReplyAsync(await _musicService.SetVolumeAsync(vol, Context.Guild.Id));

        [Command("pause")]
        public async Task Pause()
            => await ReplyAsync(await _musicService.PauseOrResumeAsync(Context.Guild.Id));

        
    }

}




