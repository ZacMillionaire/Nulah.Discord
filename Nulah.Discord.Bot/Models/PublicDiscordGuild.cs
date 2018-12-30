using System;
using System.Collections.Generic;
using System.Text;

namespace Nulah.Discord.Bot.Models {

    public class PublicDiscordGuild {
        public ulong GuildId { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; }
        public DateTime Joined { get; set; }
        public int Colour { get; set; }
    }
}
