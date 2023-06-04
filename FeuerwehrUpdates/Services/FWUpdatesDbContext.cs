using FeuerwehrUpdates.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FeuerwehrUpdates.Services
{
    public class FWUpdatesDbContext : DbContext
    {

        public FWUpdatesDbContext(DbContextOptions<FWUpdatesDbContext> options)
            : base(options) { }

        public DbSet<SubscriptionDTO> Subscriptions { get; set; }
        public DbSet<KeysDTO> Keys { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<SubscriptionDTO>().Navigation(x => x.Keys).AutoInclude();
        }

    }
}
