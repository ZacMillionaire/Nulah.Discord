using DSharpPlus.Entities;
using Nulah.Discord.MSSQL.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nulah.Discord.Bot.Management {
    public class ChannelManagement {
        private readonly string _mssqlConnectionString;

        public ChannelManagement(string mssqlConnectionString) {
            _mssqlConnectionString = mssqlConnectionString;
        }

        public List<PublicChannelShort> CreateOrUpdateChannels(Dictionary<ulong, IReadOnlyList<DiscordChannel>> channels) {

            using(var ctx = new DiscordLoggerContext(_mssqlConnectionString)) {
                var existingChannels = ctx.Channels.Select(x => new PublicChannelShort {
                    ChannelId = x.ChannelId,
                    GuildId = x.GuildId
                }).ToList();

                var existingGuilds = ctx.Guilds.ToList();

                // Loop over all the guilds first
                foreach(var guildChannel in channels) {
                    var newChannels = new List<Channel>();
                    // fuck this is stupid looking, why the fuck did I pass data in this way
                    // Loop over all the roles in the guild
                    foreach(var pendingChannel in guildChannel.Value) {
                        if(existingChannels.Any(x => x.ChannelId == pendingChannel.Id && x.GuildId == guildChannel.Key) == false) {
                            var newChannel = new Channel {
                                GuildId = guildChannel.Key,
                                ChannelId = pendingChannel.Id,
                                Created = pendingChannel.CreationTimestamp.UtcDateTime,
                                IsCategory = pendingChannel.IsCategory,
                                IsNsfw = pendingChannel.IsNSFW,
                                IsPrivate = pendingChannel.IsPrivate,
                                Name = pendingChannel.Name,
                                Position = pendingChannel.Position,
                                Topic = pendingChannel.Topic,
                                Type = pendingChannel.Type.ToString()
                            };

                            newChannel.Guild = new Guild_Channel {
                                Guild = existingGuilds.First(x => x.GuildId == guildChannel.Key),
                                Channel = newChannel
                            };

                            newChannels.Add(newChannel);
                        }
                    }
                    ctx.Channels.AddRange(newChannels);
                }

                ctx.SaveChanges();

                existingChannels.AddRange(ctx.Channels.Select(x => new PublicChannelShort {
                    ChannelId = x.ChannelId,
                    GuildId = x.GuildId
                }));

                return existingChannels;
            }
        }

        public class PublicChannelShort {
            public ulong GuildId { get; set; }
            public ulong ChannelId { get; set; }
        }
    }
}
