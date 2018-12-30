using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nulah.Discord.Bot.Modules {
    public class MessageHandler {
        private readonly string _mssqlConnectionString;
        private readonly Regex DISCORD_EMOJI_REGEX = new Regex(@"<(\w{0,}):(\w+):(\d+)>");

        public MessageHandler(string mssqlConnectionString) {
            _mssqlConnectionString = mssqlConnectionString;
        }

        public void ParseMessage(DiscordMessage discordMessage) {
            if(discordMessage.Author.IsCurrent == false && discordMessage.IsEdited == false) {
                var message = discordMessage.Content;
                var discordEmojiParse = DISCORD_EMOJI_REGEX.Matches(message).Cast<Match>().Select(x => x.Value);
                var discordEmojisRemoved = DISCORD_EMOJI_REGEX.Replace(message, string.Empty);

                var length = new System.Globalization.StringInfo(discordEmojisRemoved).LengthInTextElements;

                if(string.IsNullOrWhiteSpace(message)) {
                    // contentless message probably pure attachments
                }



                var words = Regex.Matches(discordEmojisRemoved, @"[^\s]+").Count;

                var messageDetails = new MessageDetails {
                    MessageId = discordMessage.Id,
                    GuildId = discordMessage.Channel.GuildId,
                    ChannelId = discordMessage.ChannelId,
                    UserId = discordMessage.Author.Id,
                    MessageStats = new MessageStats {
                        Length = length,
                        Words = words
                    },
                    EmojiStats = discordEmojiParse.GroupBy(x => x)
                        .Select(x => new EmojiStats {
                            Emoji = x.Key,
                            Count = x.Count()
                        })
                        .ToList(),
                    UsersMentioned = discordMessage.MentionedUsers
                        .Select(x => new UserMentioned {
                            MentionedId = x.Id,
                        })
                        .ToList(),
                    RolesMentioned = discordMessage.MentionedRoles
                        .Select(x => new RoleMentioned {
                            RoleId = x.Id
                        })
                        .ToList(),
                    Attachments = discordMessage.Attachments
                        .Select(x => new MessageAttachment {
                            ProxyUrl = x.ProxyUrl,
                            Url = x.Url,
                            Size = x.FileSize,
                            Filename = x.FileName
                        })
                        .ToList()
                };
            }
        }

        private async Task LogMessageDetailsToDatabase(MessageDetails messageDetails) {

        }

        public class MessageDetails {
            public ulong GuildId { get; set; }
            public ulong UserId { get; set; }
            public ulong ChannelId { get; set; }
            public List<MessageAttachment> Attachments { get; set; }
            public List<RoleMentioned> RolesMentioned { get; set; }
            public List<UserMentioned> UsersMentioned { get; set; }
            public List<EmojiStats> EmojiStats { get; set; }
            public MessageStats MessageStats { get; set; }
            public ulong MessageId { get; set; }
        }

        public class MessageAttachment {
            public string ProxyUrl { get; set; }
            public string Url { get; set; }
            public int Size { get; set; }
            public string Filename { get; set; }
        }

        public class RoleMentioned {
            public ulong RoleId { get; set; }
        }

        public class UserMentioned {
            public ulong MentionedId { get; set; }
        }

        public class EmojiStats {
            public string Emoji { get; set; }
            public int Count { get; set; }
        }

        public class MessageStats {
            public int Length { get; set; }
            public int Words { get; set; }
        }
    }
}
