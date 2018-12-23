using Newtonsoft.Json;
using Nulah.Discord.Bot;
using Nulah.Discord.Bot.Models;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Nulah.Discord {
    class Program {
        private static DiscordBot _dc;

        static void Main(string[] args) {
            _dc = new DiscordBot(ParseAppSettings());
            Console.Read();
        }

        static AppSettings ParseAppSettings() {
            try {
                using(FileStream fs = new FileStream($"{AppContext.BaseDirectory}/appsettings.dev.json", FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    using(StreamReader sr = new StreamReader(fs)) {
                        var contents = sr.ReadToEnd();
                        return JsonConvert.DeserializeObject<AppSettings>(contents);
                    }
                }
            } catch {
                throw;
            }
        }
    }
}
