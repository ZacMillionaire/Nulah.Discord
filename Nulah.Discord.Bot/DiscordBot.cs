using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Net.Abstractions;
using Newtonsoft.Json;
using Nulah.Discord.Bot.Commands;
using Nulah.Discord.Bot.Helpers;
using Nulah.Discord.Bot.Management;
using Nulah.Discord.Bot.Models;
using Nulah.Discord.Bot.Modules;
using Nulah.Discord.Bot.Modules.TaskRunner;
using Nulah.Discord.MSSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nulah.Discord.Bot {
    public class DiscordBot {
        private readonly DiscordClient _discordClient;

        private readonly ServerManager _serverManager;
        private readonly UserManagement _userManager;
        //private readonly PresenceManagement _presenceManager;
        //private readonly LinkManagement _linkManager;


        private readonly NulahTaskRunner _taskRunner = new NulahTaskRunner();


        public DiscordBot(AppSettings appSettings) {
            _discordClient = new DiscordClient(new DiscordConfiguration {
                Token = appSettings.DiscordBotToken,
                TokenType = TokenType.Bot,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            });

            _userManager = new UserManagement(appSettings.MSSQLConnectionString);
            //_presenceManager = new PresenceManagement(appSettings.MSSQLConnectionString);
            //_linkManager = new LinkManagement(appSettings.MSSQLConnectionString);

            _serverManager = new ServerManager(_userManager);

            //var dependency = new DependencyCollectionBuilder();
            //dependency.AddInstance(_userManager);

            //var commands = _discordClient.UseCommandsNext(new CommandsNextConfiguration {
            //    StringPrefix = "+",
            //    Dependencies = dependency.Build()
            //});

            //commands.RegisterCommands<LinkCommand>();
            //commands.RegisterCommands<RegisterCommand>();


            _discordClient.MessageCreated += _discordClient_MessageCreated;
            _discordClient.PresenceUpdated += _discordClient_PresenceUpdated;
            //_discordClient.UserUpdated += _discordClient_UserUpdated;
            _discordClient.Ready += _discordClient_Ready;
            //_discordClient.Heartbeated += _discordClient_Heartbeated;
            //_discordClient.GuildAvailable += _discordClient_GuildAvailable;
            //_discordClient.GuildMemberAdded += _discordClient_GuildMemberAdded;
            // triggers on nick changes
            //_discordClient.GuildMemberUpdated += _discordClient_GuildMemberUpdated;

            _taskRunner.RegisterTask(new UserTask(_discordClient, _serverManager), new TimeSpan(0, 0, 5));

            Task.Run(async () => {
                await Connect();
            });
        }

        private async Task _discordClient_Heartbeated(HeartbeatEventArgs e) {

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

        private async Task _discordClient_Ready(ReadyEventArgs e) {
            await _discordClient.UpdateStatusAsync(new DiscordGame("Bitcoin mining lol"));

            // Delay starting tasks until some point we can be sure we've got all discord information.
            // There's no promise that we'll have everything in n seconds time, so any registered
            // tasks relying on _discordClient should have checks in their execute methods
            // to ensure they have what they need
            _taskRunner.Start(new TimeSpan(0, 0, 0));
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
                if(e.PresenceBefore?.Game == null && e.Game == null) {
                    GenericPresenceChangeEvent(e);
                } else if(e.PresenceBefore.Game != null && e.Game == null) {
                    GameplayStopEvent(e);
                } else if(e.PresenceBefore.Game == null && e.Game != null) {
                    GameplayStartEvent(e);
                }
                await Task.Run(() => { });
            } catch(Exception ee) {
                throw;
            }
        }

        private void GameplayStartEvent(PresenceUpdateEventArgs presenceUpdate) {
            Console.WriteLine($"Game started: {presenceUpdate.Game.Name} by {presenceUpdate.Member.DisplayName}");
            var a = new {
                UserId = presenceUpdate.Member.Id,
                GuildId = presenceUpdate.Guild.Id,
                Status = "GameStart",
                Timestamp_UTC = DateTime.UtcNow,
                Game = new PublicDiscordGame {
                    Id = presenceUpdate.Game.ApplicationId,
                    Name = presenceUpdate.Game.Name,
                    Hash = StaticHelpers.GameNameToHash(presenceUpdate.Game.Name)
                }
            };
            Console.WriteLine(JsonConvert.SerializeObject(a, Formatting.Indented));
        }

        private void GameplayStopEvent(PresenceUpdateEventArgs presenceUpdate) {
            Console.WriteLine($"Game Stopped: {presenceUpdate.PresenceBefore.Game.Name} by {presenceUpdate.Member.DisplayName}");
            var a = new {
                UserId = presenceUpdate.Member.Id,
                GuildId = presenceUpdate.Guild.Id,
                Status = "GameStart",
                Timestamp_UTC = DateTime.UtcNow,
                Game = new PublicDiscordGame {
                    Id = presenceUpdate.PresenceBefore.Game.ApplicationId,
                    Name = presenceUpdate.PresenceBefore.Game.Name,
                    Hash = StaticHelpers.GameNameToHash(presenceUpdate.PresenceBefore.Game.Name)
                }
            };
            Console.WriteLine(JsonConvert.SerializeObject(a, Formatting.Indented));
        }


        private void GenericPresenceChangeEvent(PresenceUpdateEventArgs presenceUpdate) {
            var presenceEvent = new PublicPresenceEvent {
                User = new PublicDiscordUser {
                    Id = presenceUpdate.Member.Id,
                    Discriminator = int.Parse(presenceUpdate.Member.Discriminator),
                    GuildId = presenceUpdate.Member.Guild.Id,
                    Username = presenceUpdate.Member.Username
                },
                Time_UTC = DateTime.UtcNow,
                PreviousState = presenceUpdate.PresenceBefore != null
                    ? presenceUpdate.PresenceBefore.Status.ToString().ToLower() // To lower because the below Status gets the string value of the enum. Why doesn't PresenceBefore do the same? Yes.
                    : "unknown",
                NewState = presenceUpdate.Status
            };
            Console.WriteLine($"Presence change event for a registered user, {presenceUpdate.Member.DisplayName} in {presenceUpdate.Guild.Name} was {presenceEvent.PreviousState} now {presenceEvent.NewState}");
        }

        private async Task _discordClient_MessageCreated(MessageCreateEventArgs e) {
            await Task.Run(() => {
                if(e.Author.IsCurrent == false) {
                    var message = e.Message;
                    var discordEmojiParse = Regex.Matches(e.Message.Content, @"<(\w{0,}):(\w+):(\d+)>");
                    var discordEmojisRemoved = Regex.Replace(e.Message.Content, @"<\w{0,}:\w+:\d+>", string.Empty);
                    var length = new System.Globalization.StringInfo(discordEmojisRemoved).LengthInTextElements;
                    var words = Regex.Matches(discordEmojisRemoved, @"[^\s]+").Count;
                    //_linkManager.TryParseForLinks(e.Message.Content);
                    //await e.Message.RespondAsync($"echo {e.Message.Content}");
                }
            });
        }

        public void Disconnect() {
            Task.Run(async () => await _discordClient.DisconnectAsync()).Wait();
        }
    }
}
