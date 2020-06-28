using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Slab.Core
{
    internal static class RepeatingTimer
    {
        private static Timer loopingTimer;
        private static SocketGuild guild;
        internal static Task StartTimer()
        {
            guild = Global.Client.GetGuild(457603145456025612);
            loopingTimer = new Timer()
            {
                Interval = 5000,
                AutoReset = true,
                Enabled = true
            };
            loopingTimer.Elapsed += OnTimerTicked;

            return Task.CompletedTask;
        }

        private static void OnTimerTicked(object sender, ElapsedEventArgs e)
        {
            //CommandHandler.CheckIfMuted();
        }
    }
}
