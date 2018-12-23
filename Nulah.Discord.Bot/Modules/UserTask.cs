using DSharpPlus;
using Nulah.Discord.Bot.Management;
using Nulah.Discord.Bot.Models;
using Nulah.Discord.Bot.Modules.TaskRunner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.Discord.Bot.Modules {
    public class UserTask : PeriodicTask {
        private DiscordClient _discordClient;
        private ServerManager _serverManager;

        public UserTask(DiscordClient discordClient, ServerManager serverManager) {
            _discordClient = discordClient;
            _serverManager = serverManager;
        }

        public override void Execute() {
            if(_discordClient.Guilds.Count > 0) {
                _serverManager.UpdateGuildData(_discordClient.Guilds);
            }
        }
    }
}
