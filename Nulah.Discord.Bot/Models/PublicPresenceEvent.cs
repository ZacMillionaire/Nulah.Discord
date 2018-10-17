using System;
using System.Collections.Generic;
using System.Text;

namespace Nulah.Discord.Bot.Models {
    public class PublicPresenceEvent {
        public PublicDiscordUser User { get; set; }
        public DateTime Time_UTC { get; set; }
        public string PreviousState { get; set; }
        public string NewState { get; set; }
    }
}
