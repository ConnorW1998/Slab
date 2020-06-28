using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Timers;
using Slab.Core.UserAccounts;
using Slab.Core;
using Discord;

namespace Slab
{
    class CommandHandler
    {
        //Timer spamBlock;
        //List<ulong> talkedRecently = new List<ulong>();
        DiscordSocketClient _client;
        CommandService _service;

        public async Task InitializeAsync(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();
            await _service.AddModulesAsync(Assembly.GetEntryAssembly());
            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;
            if (msg == null) return;

            var context = new SocketCommandContext(_client, msg);
            int argPos = 0;

            if (msg.HasStringPrefix(Config.bot.cmdPrefix, ref argPos)
                || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                UserAccounts.updateInfo(context.User);
                var result = await _service.ExecuteAsync(context, argPos);
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }
            else
            {
                SocketUser tmpUser = context.User;
                int tmpLvl = UserAccounts.GetAccount(tmpUser).LVL;

                UserAccounts.GetAccount(context.User).XP += 1;
                UserAccounts.updateInfo(context.User);

                if (UserAccounts.GetAccount(tmpUser).LVL != tmpLvl)
                {
                    var account = UserAccounts.GetAccount(tmpUser);

                    string rank = UserAccounts.assignRank(account);


                    string title = $"Level Up!";

                    string message = $"{tmpUser.Username} is now a {rank}!\n" +
                                     $"Level: {account.LVL}";

                    var embedDef = new EmbedBuilder();
                    embedDef.WithTitle(title);
                    embedDef.WithDescription(message);
                    embedDef.WithColor(new Color(0, 255, 0));
                    embedDef.ThumbnailUrl = tmpUser.GetAvatarUrl().ToString();
                    await context.Channel.SendMessageAsync("", false, embedDef);
                }
            }
        }
    }
}