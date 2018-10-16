using Nulah.Discord.Bot;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Nulah.Discord {
    class Program {
        private static DiscordBot _dc;
        static void Main(string[] args) {

            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);


            _dc = new DiscordBot("BOT CLIENT KEY HERE");
            Console.ReadKey();
            Console.WriteLine("Shutting down");

        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e) {
            Task.Run(() => _dc.Disconnect());
        }

        // Code to try in vain to disconnect the bot, but ultimately it doesn't matter as the bot will be disconnected
        // after enough heartbeat fails from Discords end.
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig) {
            switch(sig) {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                default:
                    _dc.Disconnect();
                    Thread.Sleep(2000);
                    return false;
            }
        }
    }
}
