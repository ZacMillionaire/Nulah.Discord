using Nulah.Discord.MSSQL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.Discord.Bot.Management {
    public class PresenceManagement {
        private readonly string _mssqlConnectionString;

        public PresenceManagement(string mssqlConnectionString) {
            _mssqlConnectionString = mssqlConnectionString;
        }

        public async Task AddPresences(List<PresenceEvent> presences) {
            /*
            try {
                using(var dbctx = new DiscordContext(_mssqlConnectionString)) {
                    dbctx.AddRange(presences);
                    await dbctx.SaveChangesAsync();
                }
            } catch(Exception e) {
                Console.WriteLine(e.GetBaseException().Message);
                Console.WriteLine(e.StackTrace);
                throw;
            }*/
        }
    }
}
