using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace Nulah.Discord.Bot.Commands {
    public class LinkCommand {
        [Command("+Link")]
        public async Task CreateLink(CommandContext commandContext,
            [Description("Type to give the link. Eg: Steam, meme, passwords.txt")] string linkType,
            [Description("Name of the link in quotes")] string linkName,
            [RemainingText, Description("Link")] string link
            ) {
            var a = new {
                Guid = new {
                    Id = commandContext.Guild.Id,
                    Name = commandContext.Guild.Name
                },
                Sender = new {
                    Id = commandContext.Member.Id,
                    Name = commandContext.Member.DisplayName,
                    Discriminator = commandContext.Member.Discriminator
                }
            };
            await commandContext.Message.DeleteAsync();
            await commandContext.Member.SendMessageAsync($"{linkType} {linkName} {link}");
        }

        //[Command("+Link")]
        //public async Task CreateLink(CommandContext commandContext) {
        //    var a = new {
        //        Guid = new {
        //            Id = commandContext.Guild.Id,
        //            Name = commandContext.Guild.Name
        //        },
        //        Sender = new {
        //            Id = commandContext.Member.Id,
        //            Name = commandContext.Member.DisplayName,
        //            Discriminator = commandContext.Member.Discriminator
        //        }
        //    };
        //    await commandContext.Message.DeleteAsync();
        //    await commandContext.Member.SendMessageAsync($"Hi {commandContext.Member.DisplayName}! Usage for +Link is: +Link [type] [name] [link]");
        //}
    }
}
