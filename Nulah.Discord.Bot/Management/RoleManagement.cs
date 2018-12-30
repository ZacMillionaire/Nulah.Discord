using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Nulah.Discord.MSSQL.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nulah.Discord.Bot.Management {
    public class RoleManagement {
        private readonly string _mssqlConnectionString;

        public RoleManagement(string mssqlConnectionString) {
            _mssqlConnectionString = mssqlConnectionString;
        }

        public List<RoleShort> CreateOrUpdateRoles(Dictionary<ulong, IReadOnlyList<DiscordRole>> roles) {

            using(var ctx = new DiscordLoggerContext(_mssqlConnectionString)) {
                var existingRoles = ctx.Roles.Select(x => new RoleShort {
                    RoleId = x.RoleId,
                    GuildId = x.GuildId
                }).ToList();

                var existingGuilds = ctx.Guilds.ToList();

                // Loop over all the guilds first
                foreach(var guildRoles in roles) {
                    var newRoles = new List<Role>();
                    // fuck this is stupid looking, why the fuck did I pass data in this way
                    // Loop over all the roles in the guild
                    foreach(var pendingRole in guildRoles.Value) {
                        if(existingRoles.Any(x => x.RoleId == pendingRole.Id && x.GuildId == guildRoles.Key) == false) {
                            var newRole = new Role {
                                GuildId = guildRoles.Key,
                                Color = pendingRole.Color.Value,
                                Created = pendingRole.CreationTimestamp.UtcDateTime,
                                Name = pendingRole.Name,
                                RoleId = pendingRole.Id
                            };

                            newRole.Guild = new Guild_Role {
                                Guild = existingGuilds.First(x => x.GuildId == guildRoles.Key),
                                Role = newRole
                            };

                            newRoles.Add(newRole);
                        }
                    }
                    ctx.Roles.AddRange(newRoles);
                }

                ctx.SaveChanges();

                existingRoles.AddRange(ctx.Roles.Select(x => new RoleShort {
                    RoleId = x.RoleId,
                    GuildId = x.GuildId
                }));

                return existingRoles;
            }
        }

        public class RoleShort {
            public ulong RoleId { get; set; }
            public ulong GuildId { get; set; }
        }
    }
}
