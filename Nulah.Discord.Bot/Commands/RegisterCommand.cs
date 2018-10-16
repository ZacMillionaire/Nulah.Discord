using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Nulah.Discord.MSSQL;
using System.Linq;
using System.Threading.Tasks;

namespace Nulah.Discord.Bot.Commands {
    public class RegisterCommand {
        [Command("Register")]
        [Aliases("RegMe")]
        public async Task Register(CommandContext commandContext) {

            var newUser = new User {
                Id = commandContext.Member.Id,
                Discriminator = int.Parse(commandContext.Member.Discriminator),
                Username = commandContext.Member.Username,
                GuildId = commandContext.Member.Guild.Id
            };

            await Task.Run(async () => {
                await commandContext.Message.DeleteAsync();
                using(var dbctx = new DiscordContext()) {
                    if(dbctx.Users.Any(x => x.Id == newUser.Id && x.GuildId == newUser.GuildId) == false) {
                        dbctx.Add(newUser);
                        await dbctx.SaveChangesAsync();
                        await commandContext.Member.SendMessageAsync($"All good, you're now registered, {commandContext.Member.DisplayName}!");
                    } else {
                        await commandContext.Member.SendMessageAsync($"No need to register again, {commandContext.Member.DisplayName}, I still know who you are!");
                    }
                }
            });
        }
    }
}
