using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Nulah.Discord.Bot.Helpers;
using Nulah.Discord.Bot.Models;
using Nulah.Discord.MSSQL;
using Nulah.Discord.MSSQL.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.Discord.Bot.Management {
    public class UserManagement {
        private readonly string _mssqlConnectionString;

        public UserManagement(string mssqlConnectionString) {
            _mssqlConnectionString = mssqlConnectionString;
        }

        public List<PublicDiscordUser> CreateOrUpdateUsers(Dictionary<ulong, IReadOnlyList<DiscordMember>> members) {
            using(var ctx = new DiscordLoggerContext(_mssqlConnectionString)) {

                var newUsers = new Dictionary<ulong, User>();

                foreach(var guild in members.Keys) {

                    var pendingMembers = members[guild].Select(x => x.Id).ToArray();
                    var existingGuildMembers = ctx.Users.Where(x => pendingMembers.Contains(x.UserId)).ToList();
                    var guildDetails = ctx.Guilds.Include("Roles.Role").First(x => x.GuildId == guild);
                    // The below code is the same as the above, but is more explicit with its including
                    //var guildDetails2 = ctx.Guilds.Include(x => x.Roles).ThenInclude(x => x.Role).First(x => x.GuildId == guild);

                    foreach(var pendingMember in members[guild]) {
                        // Only add new members for now
                        if(existingGuildMembers.Any(x => x.UserId == pendingMember.Id) == false) {
                            if(newUsers.ContainsKey(pendingMember.Id) == false) {
                                var newMember = new User {
                                    AvatarUrl = pendingMember.AvatarUrl,
                                    Created = pendingMember.CreationTimestamp.UtcDateTime,
                                    Discriminator = int.Parse(pendingMember.Discriminator),
                                    IsBot = pendingMember.IsBot,
                                    UserId = pendingMember.Id,
                                    Username = pendingMember.Username,
                                    Guilds = new List<Guild_User>()
                                };

                                // Double picking from a collection. Gross, but a dictionary lookup is quicker than
                                // replacing it with pendingMember.Roles.First(y => y.Id == x.RoleId).Position
                                // as .First loops over all elements, and the select is already a loop.
                                var roles = pendingMember.Roles.Select(x => x.Id).ToArray();
                                var rolePositions = pendingMember.Roles.ToDictionary(x => x.Id, x => x.Position);
                                // Roles are sorted by their position descending - some servers use this to create
                                // pseudo groups in profiles
                                newMember.Roles = guildDetails.Roles
                                    .Where(x => roles.Contains(x.RoleId))
                                    .Select(x => new User_Role {
                                        Role = x.Role,
                                        User = newMember,
                                        RoleOrder = rolePositions[x.RoleId]
                                    })
                                    .ToList();

                                newMember.Guilds.Add(new Guild_User {
                                    Guild = guildDetails,
                                    User = newMember,
                                    Nickname = pendingMember.Nickname,
                                    Joined = pendingMember.JoinedAt.UtcDateTime,
                                    Colour = pendingMember.Color.Value
                                });
                                newUsers.Add(newMember.UserId, newMember);
                            } else {
                                newUsers[pendingMember.Id].Guilds.Add(new Guild_User {
                                    Guild = guildDetails,
                                    User = newUsers[pendingMember.Id],
                                    Nickname = pendingMember.Nickname,
                                    Joined = pendingMember.JoinedAt.UtcDateTime,
                                    Colour = pendingMember.Color.Value
                                });
                            }
                        }
                    }
                }

                try {
                    ctx.Users.AddRange(newUsers.Select(x => x.Value));

                    ctx.SaveChanges();

                    return ctx.Users.Select(x => new PublicDiscordUser {
                        Guilds = x.Guilds.Select(y => new PublicDiscordGuild {
                            GuildId = y.GuildId,
                            Name = y.Guild.Name,
                            Nickname = y.Nickname,
                            Joined = y.Joined,
                            Colour = y.Colour
                        }).ToList(),
                        Discriminator = x.Discriminator,
                        Id = x.UserId,
                        Username = x.Username,
                        Created = x.Created,
                        Roles = x.Roles.Select(y => new PublicDiscordRole {
                            Color = y.Role.Color,
                            GuildId = y.Role.GuildId,
                            Id = y.RoleId,
                            Name = y.Role.Name,
                            Position = y.RoleOrder
                        }).ToList()
                    })
                    .ToList();

                } catch {
                    throw;
                }
            }
        }

    }
}
