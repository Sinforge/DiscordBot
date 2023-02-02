using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace DiscordBot
{
    public class ApplicationDbContext: DbContext
    {
        public DbSet<DotaUser> Users { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

  
       

    }

    public class DotaUser
    {
        public ulong Id { get; set; }
        
        public ulong DiscordId { get; set; }
        public ulong DotaBuffId { get; set; }

    }
}
