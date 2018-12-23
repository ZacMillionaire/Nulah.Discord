using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nulah.Discord.Bot.Management {
    public class ServerManager {
        private readonly UserManagement _userManagement;

        public ServerManager(UserManagement userManagement) {
            _userManagement = userManagement;
        }

        public void GetUsers(Dictionary<ulong, IEnumerable<DiscordMember>> presences) {
            _userManagement.DiscordMemberToModel(presences.SelectMany(x => x.Value).ToList());
        }

        public void GetChannels(Dictionary<ulong, IReadOnlyList<DiscordChannel>> dictionary) {
        }

        public void GetRoles(Dictionary<ulong, IReadOnlyList<DiscordRole>> dictionary) {

        }

        public void UpdateGuildData(IReadOnlyDictionary<ulong, DiscordGuild> guilds) {
            GetUsers(guilds.ToDictionary(x => x.Key, x => x.Value.Members.Where(y => y.IsCurrent == false)));
            GetRoles(guilds.ToDictionary(x => x.Key, x => x.Value.Roles));
            GetChannels(guilds.ToDictionary(x => x.Key, x => x.Value.Channels));
        }
    }
}
