using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Slab.Core;

namespace Slab
{
    class Program
    {
        DiscordSocketClient _client;
        CommandHandler _handler;


        static void Main(string[] args) => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            if (Config.bot.token == "" || Config.bot.token == null) return;

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });

            _client.Log += Log;
            _client.Ready += RepeatingTimer.StartTimer;
            _client.ReactionAdded += OnReactionAdded;

            await _client.LoginAsync(TokenType.Bot, Config.bot.token);
            await _client.StartAsync();

            Global.Client = _client;
            Global.prev = new Emoji("⏮");
            Global.next = new Emoji("⏭");
            Global.exit = new Emoji("🗑");
            Global.WDRunning = false;
            //Global.versionNumber = Config.bot.version;
            //Config.UpdateConfig(Global.versionNumber);
            //Console.WriteLine($"{Global.versionNumber}");

            //! Scratch card:
            Global.scratchItems.Add(0, "Rainbow"); Global.scratchItems.Add(1, "Card"); Global.scratchItems.Add(2, "Strawberry");
            Global.scratchItems.Add(3, "Heart"); Global.scratchItems.Add(4, "Gold Bar"); Global.scratchItems.Add(5, "Grape");

            _handler = new CommandHandler();
            await _handler.InitializeAsync(_client);
            ConsoleInput();
            await Task.Delay(-1);
        }
        
        //! Re-created the messaging from console ability
        private async Task ConsoleInput()
        { 
            string input = string.Empty;
            while (input != null || input.Trim().ToLower() != "block")
            {
                input = input.Trim().ToLower();
                input = Console.ReadLine();

                if (input == "message")
                    await ConsoleSendMessage();
            }
        }

        private async Task ConsoleSendMessage()
        {
            Console.WriteLine("Select the guild:");
            var guild = GetSelectedGuild(_client.Guilds);
            var channel = GetSelectedTextChannel(guild.TextChannels);
            var message = string.Empty;
            while (message.Trim() == string.Empty)
            {
                Console.WriteLine("Your message:");
                message = Console.ReadLine();
            }

            await channel.SendMessageAsync(message);
        }

        private SocketTextChannel GetSelectedTextChannel(IEnumerable<SocketTextChannel> textChannels)
        {
            var socketChannel = textChannels.ToList();
            var maxIndex = textChannels.Count() - 1;
            for (int i = 0; i <= maxIndex; i++)
            {
                Console.WriteLine($"{i}) {textChannels.ElementAt(i).Name}");
            }

            int selectedIndex = -1;
            while (selectedIndex < 0 || selectedIndex > maxIndex)
            {
                bool success = int.TryParse(Console.ReadLine(), out selectedIndex);

                if (!success)
                {
                    Console.WriteLine("That was an invalid index, try again.");
                    selectedIndex = -1;
                }
            }

            return socketChannel[selectedIndex];
        }

        private SocketGuild GetSelectedGuild(IEnumerable<SocketGuild> guilds)
        {
            var socketGuilds = guilds.ToList();
            var maxIndex = guilds.Count() - 1;
            for (int i = 0; i <= maxIndex; i++)
            {
                Console.WriteLine($"{i}) {guilds.ElementAt(i).Name}");
            }

            int selectedIndex = -1;
            while (selectedIndex < 0 || selectedIndex > maxIndex)
            {
                bool success = int.TryParse(Console.ReadLine(), out selectedIndex);

                if (!success)
                {
                    Console.WriteLine("That was an invalid index, try again.");
                    selectedIndex = -1;
                }
            }

            return socketGuilds[selectedIndex];
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.MessageId == Global.MessageTrackingId)
            {
                if (reaction.Emote.Name == Global.prev.Name)
                {
                    await channel.SendMessageAsync("Prev Menu.");
                }
                if (reaction.Emote.Name == Global.next.Name)
                {
                    await channel.SendMessageAsync("Next Menu.");
                }
                if (reaction.Emote.Name == Global.next.Name)
                {
                    await channel.SendMessageAsync("Exit Menu.");
                }
            }
        }

        private async Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
        }
    }
}
