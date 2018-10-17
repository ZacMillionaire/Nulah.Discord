using Nulah.Discord.Bot.Models;
using Nulah.Discord.MSSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.Discord.Bot.Management {
    public class UserManagement {
        private Dictionary<ulong, List<ulong>> _registeredUsers = new Dictionary<ulong, List<ulong>>();
        private readonly string _mssqlConnectionString;

        public UserManagement(string mssqlConnectionString) {
            _mssqlConnectionString = mssqlConnectionString;
            GetRegisteredUsers();
        }

        /// <summary>
        /// Populates all users registered into a dictionary
        /// </summary>
        private void GetRegisteredUsers() {
            using(var dbctx = new DiscordContext(_mssqlConnectionString)) {
                _registeredUsers = dbctx.Users.GroupBy(x => x.Id)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.GuildId).ToList());

            }
        }

        public bool UserIsRegistered(ulong userId, ulong guildId) {
            return _registeredUsers.ContainsKey(userId) && _registeredUsers[userId].Contains(guildId);
        }

        private void AddNewRegisteredUser(User newUser) {
            // If this is a user we've registered before, just add the new guildId to their list
            if(_registeredUsers.ContainsKey(newUser.Id) == true) {
                _registeredUsers[newUser.Id].Add(newUser.GuildId);
            } else {
                // Otherwise, create a new user entry with the new guildId
                _registeredUsers.Add(newUser.Id, new List<ulong>() { newUser.GuildId });
            }
        }

        public async Task<bool> RegisterUser(User newUser) {
            try {
                using(var dbctx = new DiscordContext(_mssqlConnectionString)) {
                    if(UserIsRegistered(newUser.Id, newUser.GuildId) == false) {
                        dbctx.Add(newUser);
                        await dbctx.SaveChangesAsync();
                        AddNewRegisteredUser(newUser);
                        return true;
                    } else {
                        return false;
                    }
                }
            } catch(Exception e) {
                Console.WriteLine(e.GetBaseException().Message);
                Console.WriteLine(e.StackTrace);
                throw;
            }
        }
    }
}
