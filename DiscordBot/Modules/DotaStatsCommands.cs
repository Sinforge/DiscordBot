using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.Services;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace DiscordBot.Modules
{
    public class DotaStatsCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ApplicationDbContext _db;
        private readonly DotaStatsService _dotaStatsService;

        public DotaStatsCommands(ApplicationDbContext db, DotaStatsService dotaStatsService)
        {
            _dotaStatsService = dotaStatsService;
            _db = db;
        }

        [Command("reg")]
        public async Task Registration(ulong? dotabuffId)
        {
            if (dotabuffId == null)
            {
                await ReplyAsync("You should enter correct not null dotabuff id");
                return;
            }

            IEnumerable<DotaUser> sameId = from dbId in _db.Users.Where(
                u => (u.DotaBuffId == dotabuffId || u.DiscordId == Context.User.Id)) select dbId;
            if (sameId.Any())
            {
                await ReplyAsync("User with this id exist");
                return;
            }

            try
            {
                _db.Add(new DotaUser { DotaBuffId = (ulong)dotabuffId, DiscordId = Context.User.Id });
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                await ReplyAsync("Sorry, problems with database");
            }
            finally
            {
                await ReplyAsync("Your account successful registered");

            }

        }

        [Command("unlink")]
        public async Task Unlink()
        {
            ulong? userId = Context.User.Id;
            DotaUser? dotaUserDb = _db.Users.FirstOrDefault(u => u.DiscordId == userId);
            if (dotaUserDb == null)
            {
                await ReplyAsync("You should be registered before unlink");
                return;
            }

            _db.Remove(dotaUserDb);
            await _db.SaveChangesAsync();
            await ReplyAsync("Your account successful unlinked");

        }


        [Command("most-played-heroes", RunMode = RunMode.Async)]
        public async Task GetHeroes(IUser user)
        {
            DotaUser dotaUser;
            if (user == null)
            {
                dotaUser = await _db.Users.FirstOrDefaultAsync(u => u.DiscordId == Context.User.Id);
            }
            else
            {
                dotaUser = await _db.Users.FirstOrDefaultAsync(u => u.DiscordId == user.Id);
            }

            IUser currentUser = Context.Guild.Users.FirstOrDefault(u => u.Id == dotaUser.DiscordId);

            Console.WriteLine("Я получаю данные с дотабафа");
            List<string> listHeroes = await _dotaStatsService.GetMostPlayedHeroes(dotaUser.DotaBuffId);
            StringBuilder info = new StringBuilder("Hero-Matches-WinRate-KDA-Role-Lane\n");
            foreach (var data in listHeroes)
            {
                info.AppendLine(data);
            }

            Console.WriteLine(info.ToString());
            var embedBuilder = new EmbedBuilder()
                .WithAuthor(currentUser.ToString()
                    , currentUser.GetAvatarUrl() ?? currentUser.GetDefaultAvatarUrl())
                .WithTitle("Most played heroes")
                .WithDescription(info.ToString())
                .WithColor(Color.Purple)
                .WithCurrentTimestamp();
            await ReplyAsync(embed: embedBuilder.Build());
        }

    }
}
