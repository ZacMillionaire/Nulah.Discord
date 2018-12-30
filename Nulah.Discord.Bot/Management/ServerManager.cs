using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.Discord.Bot.Management {
    public class ServerManager {
        private readonly UserManagement _userManagement;
        private readonly GuildManagement _guildManagement;
        private readonly RoleManagement _roleMangement;
        private readonly ChannelManagement _channelManagement;

        public ServerManager(UserManagement userManagement, GuildManagement guildManagement, RoleManagement roleManagement, ChannelManagement channelManagement) {
            _userManagement = userManagement;
            _guildManagement = guildManagement;
            _roleMangement = roleManagement;
            _channelManagement = channelManagement;
        }

        public void CreateOrUpdateGuilds(List<DiscordGuild> guilds) {
            _guildManagement.CreateOrUpdateGuilds(guilds);
        }

        private void CreateOrUpdateUsers(Dictionary<ulong, IReadOnlyList<DiscordMember>> members) {
            _userManagement.CreateOrUpdateUsers(members);
        }

        public void CreateOrUpdateChannels(Dictionary<ulong, IReadOnlyList<DiscordChannel>> channels) {
            _channelManagement.CreateOrUpdateChannels(channels);
        }

        public void CreateOrUpdateRoles(Dictionary<ulong, IReadOnlyList<DiscordRole>> roles) {
            _roleMangement.CreateOrUpdateRoles(roles);
        }

        public void UpdateGuildData(IReadOnlyDictionary<ulong, DiscordGuild> guilds) {
            try {
                CreateOrUpdateGuilds(guilds.Select(x => x.Value).ToList());
                CreateOrUpdateRoles(guilds.ToDictionary(x => x.Key, x => x.Value.Roles));
                CreateOrUpdateChannels(guilds.ToDictionary(x => x.Key, x => x.Value.Channels));
                CreateOrUpdateUsers(guilds.ToDictionary(x => x.Key, x => x.Value.Members));
            } catch(Exception e) {
                throw;
            }
        }
    }
}
