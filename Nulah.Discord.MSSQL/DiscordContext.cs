using Microsoft.EntityFrameworkCore;

namespace Nulah.Discord.MSSQL {
    public class DiscordContext : DbContext {
        public DbSet<User> Users { get; set; }
        public DbSet<PresenceEvent> PresenceEvents { get; set; }

        private readonly string _sqlConnectionString;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer(_sqlConnectionString);
        }

        public DiscordContext(string connectionString) {
            _sqlConnectionString = connectionString;
        }

        public DiscordContext() {
            _sqlConnectionString = "YOUR_DB_CONNECTION_STRING_HERE";
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<User>()
                .HasKey(x => new { x.Id, x.GuildId });
        }
    }
}
