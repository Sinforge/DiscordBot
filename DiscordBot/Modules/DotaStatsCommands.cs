using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Modules
{
    public class DotaStatsCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ApplicationDbContext _db;

        public DotaStatsCommands(ApplicationDbContext db)
        {
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

            IEnumerable<DotaUser> sameId = from dbId in _db.Users.Where(u => u.DotaBuffId == dotabuffId) select dbId;
            if (sameId.Any())
            {
                await ReplyAsync("User with this id exist");
                return;
            }

            try
            {
                _db.Add(new DotaUser { DotaBuffId = (ulong)dotabuffId });
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

    }
}
