using FeuerwehrUpdates.DTOs;
using FeuerwehrUpdates.Models;
using Microsoft.EntityFrameworkCore;

namespace FeuerwehrUpdates.Services
{
    public class FWUpdatesDbContext : DbContext
    {

        public FWUpdatesDbContext(DbContextOptions<FWUpdatesDbContext> options)
            : base(options) { }

        public DbSet<SubscriptionDTO> Subscriptions { get; set; }
        public DbSet<KeysDTO> Keys { get; set; }
        public DbSet<Einsatz> Einsaetze { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<SubscriptionDTO>().Navigation(x => x.Keys).AutoInclude();
        }

    }
}
