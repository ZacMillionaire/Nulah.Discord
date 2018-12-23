using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nulah.Discord.MSSQL {
    public class User {
        public ulong Id { get; set; }
        public int Discriminator { get; set; }
        public string Username { get; set; }
        public ulong GuildId { get; set; }
        public string Nickname { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp_UTC { get; set; }
        public string Colour { get; set; }
        public ICollection<User_Roles> Roles { get; set; }
    }

    public class Role {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ulong RoleId { get; set; }
        public string Color { get; set; }
    }

    public class User_Roles {
        public int UserId { get; set; }
        public User User { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }

    public class PresenceEvent {
        // EF Core will complain if you have a table without a key so...yeah, here one is
        [Key]
        public int Id { get; set; }
        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp_UTC { get; set; }
        public PresenceEvent_Game Game { get; set; }
    }

    public class PresenceEvent_Game {
        public int PresenceEventId { get; set; }
        public PresenceEvent PresenceEvent { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }
    }

    public class Game {
        [Key]
        public int Id { get; set; }
        public string GameHash { get; set; } // used to track all games as discord does not assign application ids to all of them
        public ulong? GameId { get; set; }
        public string Name { get; set; }
    }

}
