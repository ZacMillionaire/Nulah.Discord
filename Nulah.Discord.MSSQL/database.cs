using System;
using System.ComponentModel.DataAnnotations;

namespace Nulah.Discord.MSSQL {
    public class User {
        public ulong Id { get; set; }
        public int Discriminator { get; set; }
        public string Username { get; set; }
        public ulong GuildId { get; set; }
    }

    public class PresenceEvent {
        // EF Core will complain if you have a table without a key so...yeah, here one is
        [Key]
        public int Id { get; set; }
        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp_UTC { get; set; }
        public string GameName { get; set; }
        public ulong? GameId { get; set; }
    }
}
