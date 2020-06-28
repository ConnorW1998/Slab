using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slab.Modules.Games;

namespace Slab
{
    internal static class Global
    {
        internal static DiscordSocketClient Client { get; set; }
        internal static SocketUser user { get; set; }
        internal static ulong MessageTrackingId { get; set; }
        internal static bool WDRunning { get; set; }
        internal static int playerCount { get; set; }
        internal static List<SocketUser> players { get; set; }
        //! Scratch cards:
        internal static Dictionary<int, string> scratchItems = new Dictionary<int, string>();

        //! Emoji keepsakes:

        //! Menu Emoji's
        internal static IEmote prev;
        internal static IEmote next;
        internal static IEmote exit;

        //! Multiple Choice?:
        internal static Emoji one = new Emoji("1");
        internal static Emoji two = new Emoji("2");
        internal static Emoji three = new Emoji("3");
        internal static Emoji four = new Emoji("4");
    }
}
