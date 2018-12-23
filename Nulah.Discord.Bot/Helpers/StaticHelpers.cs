using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Nulah.Discord.Bot.Helpers {
    static class StaticHelpers {
        public static string GameNameToHash(string gameName) {
            byte[] byteArray = Encoding.UTF8.GetBytes(gameName);
            SHA512 shaM = new SHA512Managed();
            var result = shaM.ComputeHash(byteArray);
            var hash = string.Join(string.Empty, result.Select(x => x.ToString("x2")));

            return hash;
        }
    }
}
