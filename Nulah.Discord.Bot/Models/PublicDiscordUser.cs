using System;
using System.Collections.Generic;
using System.Text;

namespace Nulah.Discord.Bot.Models {
    public class PublicDiscordUser {
        public ulong Id { get; set; }
        public int Discriminator { get; set; }
        public string Username { get; set; }
        public ulong GuildId { get; set; }
        public string Nickname { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp_UTC { get; set; }
        public string Colour { get; set; }
        public List<PublicDiscordRole> Roles { get; set; }
        public PublicDiscordGame Game { get; set; }
    }
}
