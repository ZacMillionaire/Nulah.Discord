using System;
using System.Collections.Generic;
using System.Text;

namespace Nulah.Discord.Bot.Models {
    public class PublicDiscordRole {
        public string Name { get; set; }
        public ulong Id { get; set; }
        public int Color { get; set; }
        public ulong GuildId { get; set; }
        /// <summary>
        /// Determines the order to display roles, by descending order. Some servers use roles
        /// in a psuedo-group, so the position is important to replicate that ordering.
        /// </summary>
        public int Position { get; set; }
    }
}
