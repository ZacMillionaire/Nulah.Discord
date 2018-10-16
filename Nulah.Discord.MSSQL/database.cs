using System;
using System.ComponentModel.DataAnnotations;

namespace Nulah.Discord.MSSQL {
    public class User {
        [Key]
        public ulong Id { get; set; }
        public int Discriminator { get; set; }
        public string Username { get; set; }
        public ulong GuildId { get; set; }
    }

    // This name is a bit iffy but I couldn't think of better
    public class GamePlaytime {
        [Key]
        public Guid Id { get; set; }
        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }
        public string GameName { get; set; }
        public DateTime Start_UTC { get; set; }
        public DateTime End_UTC { get; set; }
    }
}
