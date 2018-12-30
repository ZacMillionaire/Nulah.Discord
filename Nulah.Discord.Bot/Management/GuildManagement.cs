using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Nulah.Discord.MSSQL.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.Discord.Bot.Management {
    public class GuildManagement {
        private readonly string _mssqlConnectionString;

        public GuildManagement(string mssqlConnectionString) {
            _mssqlConnectionString = mssqlConnectionString;
        }

        public List<ulong> CreateOrUpdateGuilds(List<DiscordGuild> guildList) {
            using(var ctx = new DiscordLoggerContext(_mssqlConnectionString)) {

                var existingGuilds = ctx.Guilds.Select(x => x.GuildId).ToList();
                var a = ctx.Guilds.Include("Users").Include("Channels").Include("Roles").ToList();

                foreach(var pendingGuild in guildList) {
                    if(existingGuilds.Any(x => x == pendingGuild.Id) == false) {

                        var newGuild = new Guild {
                            Created = pendingGuild.CreationTimestamp.UtcDateTime,
                            GuildId = pendingGuild.Id,
                            IconUrl = pendingGuild.IconUrl ?? "",
                            Name = pendingGuild.Name
                        };

                        ctx.Guilds.Add(newGuild);
                    } else {
                        UpdateGuild(ctx.Guilds.First(x => x.GuildId == pendingGuild.Id), pendingGuild);
                    }
                }

                try {
                    ctx.SaveChanges();

                    existingGuilds.AddRange(ctx.Guilds.Select(x => x.GuildId));

                    return existingGuilds;
                } catch(Exception e) {
                    throw;
                }
            }
        }

        private void UpdateGuild(Guild existingGuild, DiscordGuild pendingGuild) {

            if(existingGuild.IconUrl != pendingGuild.IconUrl) {
                existingGuild.IconUrl = pendingGuild.IconUrl;
            }

            if(existingGuild.Name != pendingGuild.Name) {
                existingGuild.Name = pendingGuild.Name;
            }

        }
    }
}
