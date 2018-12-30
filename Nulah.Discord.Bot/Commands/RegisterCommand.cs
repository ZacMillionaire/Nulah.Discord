using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Nulah.Discord.Bot.Management;
using Nulah.Discord.MSSQL;
using Nulah.Discord.MSSQL.DbModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Nulah.Discord.Bot.Commands {
    public class RegisterCommand {
        private readonly UserManagement _userManagement;

        public RegisterCommand(UserManagement userManagement) {
            _userManagement = userManagement;
        }

        [Command("Register")]
        [Aliases("RegMe")]
        public async Task Register(CommandContext commandContext) {
            throw new NotImplementedException();
            /*
            var newUser = new User {
                UserId = commandContext.Member.Id,
                Discriminator = int.Parse(commandContext.Member.Discriminator),
                Username = commandContext.Member.Username,
                GuildId = commandContext.Member.Guild.Id
            };

            await Task.Run(async () => {
                await commandContext.Message.DeleteAsync();
                var userRegistered = await _userManagement.RegisterUser(newUser);

                if(userRegistered == true) {
                    await commandContext.Member.SendMessageAsync($"All good, you're now registered for {commandContext.Member.Guild.Name}, {commandContext.Member.DisplayName}!" +
                        $"From now on, I'll track your channel activity including whatever you might be playing as best I can!");
                } else {
                    await commandContext.Member.SendMessageAsync($"No need to register for {commandContext.Member.Guild.Name} again, {commandContext.Member.DisplayName}, I still know who you are!");
                }
            });*/
        }
    }
}
