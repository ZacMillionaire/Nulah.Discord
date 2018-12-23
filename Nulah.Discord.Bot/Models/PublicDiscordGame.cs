using System;
using System.Collections.Generic;
using System.Text;

namespace Nulah.Discord.Bot.Models {
    public class PublicDiscordGame {
        public ulong? Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Used to uniquely identify games as not all games will have an Id.
        /// </summary>
        public string Hash { get; set; }
    }
}
