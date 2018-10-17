using System;
using System.Collections.Generic;
using System.Text;

namespace Nulah.Discord.Bot.Models {
    public class PublicDiscordUser {
        public ulong Id { get; set; }
        public int Discriminator { get; set; }
        public string Username { get; set; }
        public ulong GuildId { get; set; }
    }
}
