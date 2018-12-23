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
            //_sqlConnectionString = "Data Source=192.168.1.101,1433\\HOMEEXPRESS;Initial Catalog=NulahDiscord_Dev;Integrated Security=False;User ID=HomeExpress;Password=P@ssw0rd;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";
            _sqlConnectionString = "Server=localhost\\SQLEXPRESS;Database=DiscordLogger;Trusted_Connection=True;";
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<User>()
                .HasKey(x => new { x.Id, x.GuildId });
        }
    }
}
