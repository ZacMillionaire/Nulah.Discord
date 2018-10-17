using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Net.Abstractions;
using Nulah.Discord.Bot.Commands;
using Nulah.Discord.Bot.Management;
using Nulah.Discord.Bot.Models;
using Nulah.Discord.MSSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Nulah.Discord.Bot {
    public class DiscordBot {
        private readonly DiscordClient _discordClient;
        //private readonly Timer _t;

        private readonly UserManagement _userManager;
        private readonly PresenceManagement _presenceManager;

        public DiscordBot(AppSettings appSettings) {
            _discordClient = new DiscordClient(new DiscordConfiguration {
                Token = appSettings.DiscordBotToken,
                TokenType = TokenType.Bot,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            });

            _userManager = new UserManagement(appSettings.MSSQLConnectionString);
            _presenceManager = new PresenceManagement(appSettings.MSSQLConnectionString);

            var dependency = new DependencyCollectionBuilder();
            dependency.AddInstance(_userManager);

            var commands = _discordClient.UseCommandsNext(new CommandsNextConfiguration {
                StringPrefix = "+",
                Dependencies = dependency.Build()
            });

            commands.RegisterCommands<LinkCommand>();
            commands.RegisterCommands<RegisterCommand>();


            //_discordClient.MessageCreated += _discordClient_MessageCreated;
            //_discordClient.PresenceUpdated += _discordClient_PresenceUpdated;
            //_discordClient.UserUpdated += _discordClient_UserUpdated;
            _discordClient.Ready += _discordClient_Ready;
            _discordClient.Heartbeated += _discordClient_Heartbeated;
            //_discordClient.GuildAvailable += _discordClient_GuildAvailable;
            //_discordClient.GuildMemberAdded += _discordClient_GuildMemberAdded;
            // triggers on nick changes
            //_discordClient.GuildMemberUpdated += _discordClient_GuildMemberUpdated;

            Task.Run(async () => {
                await Connect();
            });

        }

        private async Task _discordClient_Heartbeated(HeartbeatEventArgs e) {
            var utcNow = DateTime.UtcNow;
            await Task.Run(async () => {
                if(e.Client.Guilds.Count > 0) {
                    var presences = e.Client.Guilds.Select(x => new {
                        Presences = x.Value.Members.Where(y => y.IsCurrent == false && _userManager.UserIsRegistered(y.Id, x.Key))
                            .Select(y => new PresenceEvent {
                                UserId = y.Id,
                                GuildId = x.Key,
                                Status = ( y.Presence == null ) ? "offline" : y.Presence.Status.ToString().ToLower(),
                                GameId = y.Presence?.Game?.ApplicationId,
                                GameName = y.Presence?.Game?.Name,
                                Timestamp_UTC = utcNow
                            })
                    })
                    .SelectMany(x => x.Presences).ToList();

                    await _presenceManager.AddPresences(presences);
                }
            });
        }

        /// <summary>
        /// Can be used for nickname changes
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task _discordClient_GuildMemberUpdated(GuildMemberUpdateEventArgs e) {
            await Task.Run(() => {

            });
        }
        /*
        // Old auto register code
        private async Task _discordClient_GuildMemberAdded(GuildMemberAddEventArgs e) {
            var newUser = new User {
                Id = e.Member.Id,
                Discriminator = int.Parse(e.Member.Discriminator),
                Username = e.Member.Username
            };

            await Task.Run(async () => {
                using(var dbctx = new DiscordContext()) {
                    if(dbctx.Users.Any(x => x.Id == newUser.Id) == false) {
                        dbctx.Add(newUser);
                        await dbctx.SaveChangesAsync();
                    }
                }
            });
        }

        private async Task _discordClient_GuildAvailable(GuildCreateEventArgs e) {
            var presences = _discordClient.Presences.Values
                .Where(x => x.User != _discordClient.CurrentUser && x.User.IsBot == false)
                .Select(x => new User {
                    Id = x.User.Id,
                    Username = x.User.Username,
                    Discriminator = int.Parse(x.User.Discriminator)
                });

            await Task.Run(async () => {
                using(var dbctx = new DiscordContext()) {
                    var existingUsers = dbctx.Users.Select(x => x.Id);
                    var intersection = presences.Select(x => x.Id)
                        .Intersect(existingUsers);
                    var newUsers = presences.Where(x => intersection.Contains(x.Id) == false);
                    dbctx.Users.AddRange(newUsers);
                    await dbctx.SaveChangesAsync();
                }
            });
        }
*/

        private async Task _discordClient_Ready(ReadyEventArgs e) {
            await _discordClient.UpdateStatusAsync(new DiscordGame("Bitcoin mining lol"));
        }

        private async Task Connect() {
            await _discordClient.ConnectAsync();

            //_t = new Timer(new TimerCallback(GetDetails), null, new TimeSpan(0, 0, 1), new TimeSpan(0, 0, 1));
        }

        //private void GetDetails(object o) {

        //}

        private async Task _discordClient_UserUpdated(UserUpdateEventArgs e) {
            await Task.Run(() => {

            });
        }

        private async Task _discordClient_PresenceUpdated(PresenceUpdateEventArgs e) {
            try {
                if(e.Game == null || e.PresenceBefore.Game == null) {
                    GenericPresenceChangeEvent(e);
                } else if(e.PresenceBefore.Game != null) {
                    GameplayStopEvent(e);
                } else if(e.Game != null) {
                    GameplayStartEvent(e);
                }
                await Task.Run(() => { });
            } catch(Exception ee) {
                throw;
            }
            /*
            // Timestamp is broken and won't actually return me a valid DateTime, despite having a _start value
            // ...as an internal field.
            // Well fuck that, if it exists, I can get it out.
            TransportGame tg;
            // Flags we need to pull the field. It's internal, so all we need is a nonpublic instance binding flag
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            // If PresenceUpdateEventArgs.Game isn't null, a user just started to play a game,
            // otherwise, they finished playing a game, and what would be in PresenceUpdateEventArgs.Game is now in
            // PresenceUpdateEventArgs.PresenceBefore.Game
            if(e.Game != null) {
                tg = e.Game;
            } else {
                tg = e.PresenceBefore.Game;
            }

            // Get the field info for the internal field _start
            FieldInfo fi = tg.Timestamps.GetType().GetField("_start", flags);
            // Then get the value as a nullable long, can't cast an object to a primitive without declaring it nullable
            // and there's the potential for _start to infact be null.
            var start = fi.GetValue(tg.Timestamps) as long?;

            // If we have a start value we can carry on carefree (mostly)
            if(start != null) {
                var dictKey = $"{e.Member.Id}:{e.Guild.Id}:{tg.Name}:{start}";

                if(e.Game != null && _currentGameTracker.ContainsKey(dictKey) == false) {
                    _currentGameTracker.Add(dictKey, Guid.NewGuid());
                }

                if(e.PresenceBefore.Game != null) {
                    var gameStatus = new GamePlaytime {
                        UserId = e.Member.Id,
                        //GuildId = e.Guild.Id,
                        GameName = tg.Name,
                        Start_UTC = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds((double)start), // we can be 100% sure we have a value for start, so its safe to cast to a long
                        End_UTC = DateTime.UtcNow,
                        Id = _currentGameTracker[dictKey]
                    };
                    await Task.Run(async () => {
                        using(var dbctx = new DiscordContext()) {
                            dbctx.GamePlaytimes.Add(gameStatus);
                            await dbctx.SaveChangesAsync();
                            _currentGameTracker.Remove(dictKey);
                        }
                    });
                }

            }*/
        }

        private void GameplayStartEvent(PresenceUpdateEventArgs presenceUpdate) {
        }

        private void GameplayStopEvent(PresenceUpdateEventArgs presenceUpdate) {
        }

        private void GenericPresenceChangeEvent(PresenceUpdateEventArgs presenceUpdate) {
            if(_userManager.UserIsRegistered(presenceUpdate.Member.Id, presenceUpdate.Member.Guild.Id)) {
                var presenceEvent = new PublicPresenceEvent {
                    User = new PublicDiscordUser {
                        Id = presenceUpdate.Member.Id,
                        Discriminator = int.Parse(presenceUpdate.Member.Discriminator),
                        GuildId = presenceUpdate.Member.Guild.Id,
                        Username = presenceUpdate.Member.Username
                    },
                    Time_UTC = DateTime.UtcNow,
                    PreviousState = presenceUpdate.PresenceBefore.Status.ToString().ToLower(), // The fuck is this different to the new presence below
                    NewState = presenceUpdate.Status
                };
                Console.WriteLine($"Presence change event for a registered user, {presenceUpdate.Member.DisplayName} in {presenceUpdate.Guild.Name}");
            } else {
                Console.WriteLine($"Presence change event for an unregistered user, {presenceUpdate.Member.DisplayName} in {presenceUpdate.Guild.Name}");
            }
        }

        private async Task _discordClient_MessageCreated(MessageCreateEventArgs e) {
            if(e.Author.IsCurrent == false) {
                await e.Message.RespondAsync($"echo {e.Message.Content}");
            }
        }
        public void Disconnect() {
            Task.Run(async () => await _discordClient.DisconnectAsync()).Wait();
        }
    }
}
