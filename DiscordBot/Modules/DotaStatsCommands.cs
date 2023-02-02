using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.Services;
using Microsoft.EntityFrameworkCore;

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


        [Command("most-played-heroes")]
        public async Task GetHeroes(IUser user = null)
        {
            ulong dotabuffId;
            if (user == null)
            {
                dotabuffId = _db.Users.FirstOrDefault(u => u.DiscordId == Context.User.Id).DotaBuffId;
            }
            else
            {
                dotabuffId = _db.Users.FirstOrDefault(u => u.DiscordId == user.Id).DotaBuffId;
            }

            Console.WriteLine(dotabuffId);

            _dotaStatsService.GetMostPlayedHeroes(dotabuffId);
        }

    }
}
