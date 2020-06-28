using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Slab.Core.UserAccounts;
using Slab.Modules.WDfiles;
using Slab.Modules.Games;

namespace Slab.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        [Command("Test")]
        [RequireOwner]
        public async Task Test([Remainder] string arg = "")
        {
            SocketUser author = Context.Message.Author;
            string[] args = arg.Split(' '); //! Seperate multiple args

            var embed = new EmbedBuilder();
            embed.WithTitle("Lobby Message:");
            embed.WithColor(new Color(0, 255, 0));

            if (args[0].ToLower() == "create" || args[0].ToLower() == "new")
            {
                Lobby createdLobby = new Lobby
                {
                    Owner = author,
                    PlayerCapacity = 10,
                    Players = new List<SocketUser>()
                };
                LobbyManager.AddPlayer(createdLobby, author);

                try
                {
                    if(args[1] != null)
                    {
                        LobbyManager.ChangeTitle(args[1], author);
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    LobbyManager.ChangeTitle($"{author.Username}'s lobby", author);
                }

                List<SocketUser> additionalPlayers = null;
                if (Context.Message.MentionedUsers.Count >= 1)
                {
                    additionalPlayers = new List<SocketUser>();
                    foreach (SocketUser su in Context.Message.MentionedUsers)
                    {

                        additionalPlayers.Add(su);
                    }
                }

                try
                {
                    if(args[2] != null)
                    {
                        LobbyManager.SetCapacity(author, Convert.ToInt32(args[2]));
                    }
                    embed.WithDescription($"Created lobby: {createdLobby.GameTitle}");
                    embed.WithFooter($"{createdLobby.CurrentPlayers} / {createdLobby.PlayerCapacity} users", "https://i.imgur.com/9Y0vuCO.png");
                }
                catch
                {
                    LobbyManager.SetCapacity(author, 10);
                    embed.WithDescription($"Created lobby: {createdLobby.GameTitle}");
                    embed.WithFooter($"{createdLobby.CurrentPlayers} / {createdLobby.PlayerCapacity} users", "https://i.imgur.com/9Y0vuCO.png");
                }
            }
            if(args[0].ToLower() == "join")
            {
                if(Context.Message.MentionedUsers.Count >= 1)
                {
                    try
                    {
                        Lobby target = LobbyManager.FindLobby(Context.Message.MentionedUsers.FirstOrDefault<SocketUser>());
                        LobbyManager.AddPlayer(target, author);

                        embed.WithDescription($"{author.Username} joined {target.Owner.Username}'s lobby: {target.GameTitle}");
                        embed.AddInlineField("In-lobby players:", target.Players);
                        embed.WithFooter($"{target.CurrentPlayers} / {target.PlayerCapacity} users", "https://i.imgur.com/9Y0vuCO.png");
                    }
                    catch (NullReferenceException)
                    {
                        await Context.Channel.SendMessageAsync("No lobby exists! Creating new lobby.");
                        List<SocketUser> additionalPlayers = null;

                        if (Context.Message.MentionedUsers.Count >= 1)
                        {
                            additionalPlayers = new List<SocketUser>();
                            foreach (SocketUser su in Context.Message.MentionedUsers)
                            {

                                additionalPlayers.Add(su);
                            }
                        }

                        LobbyManager.AddLobby(author, $"{author.Username}'s lobby", additionalPlayers);
                        Lobby target = LobbyManager.FindLobby(author);

                        embed.WithDescription($"{author.Username} created a lobby.");
                        embed.AddInlineField("In-lobby players:", target.Players);
                        embed.WithFooter($"{target.CurrentPlayers} / {target.PlayerCapacity} users", "https://i.imgur.com/9Y0vuCO.png");
                    }
                }
                else
                {
                    try
                    {
                        Lobby target = LobbyManager.FindLobby(args[1]);
                        LobbyManager.AddPlayer(target, author);

                        embed.WithDescription($"{author.Username} joined {target.Owner.Username}'s lobby: {target.GameTitle}");
                        embed.AddInlineField("In-lobby players:", target.Players);
                        embed.WithFooter($"{target.CurrentPlayers} / {target.PlayerCapacity} users", "https://i.imgur.com/9Y0vuCO.png");
                    }
                    catch (NullReferenceException)
                    {
                        await Context.Channel.SendMessageAsync("No lobby exists! Creating new lobby.");
                        List<SocketUser> additionalPlayers = null;

                        if (Context.Message.MentionedUsers.Count >= 1)
                        {
                            additionalPlayers = new List<SocketUser>();
                            foreach (SocketUser su in Context.Message.MentionedUsers)
                            {

                                additionalPlayers.Add(su);
                            }
                        }

                        LobbyManager.AddLobby(author, args[1], additionalPlayers);
                        Lobby target = LobbyManager.FindLobby(author);

                        embed.WithDescription($"{author.Username} created a lobby.");
                        embed.AddInlineField("In-lobby players:", target.Players);
                        embed.WithFooter($"{target.CurrentPlayers} / {target.PlayerCapacity} users", "https://i.imgur.com/9Y0vuCO.png");
                    }
                }
                
            }
            if(args[0].ToLower() == "leave" || args[0].ToLower() == "exit")
            {
                try
                {
                    Lobby target = LobbyManager.FindLobby(author);
                    LobbyManager.RemovePlayer(target, author);

                    embed.WithDescription($"{author.Username} left {target.Owner.Username}'s lobby: {target.GameTitle}");
                    embed.AddInlineField("In-lobby players:", target.Players);
                    embed.WithFooter($"{target.CurrentPlayers} / {target.PlayerCapacity} users", "https://i.imgur.com/9Y0vuCO.png");
                }
                catch (NullReferenceException)
                {
                    await Context.Channel.SendMessageAsync("You're not in a lobby!");
                }
            }
            if(args[0].ToLower() == "edit" || args[0].ToLower() == "change")
            {
                Lobby target = LobbyManager.FindLobby(author);
                if (args[1].ToLower() == "cap" || args[1].ToLower() == "capacity")
                {
                    LobbyManager.SetCapacity(author, Convert.ToInt32(args[2]));

                    embed.WithDescription($"{author.Username} changed his lobby's capacity to: {target.PlayerCapacity}");
                    embed.AddInlineField("In-lobby players:", target.Players);
                    embed.WithFooter($"{target.CurrentPlayers} / {target.PlayerCapacity} users", "https://i.imgur.com/9Y0vuCO.png");
                }
                else if (args[1].ToLower() == "name" || args[1].ToLower() == "title")
                {
                    LobbyManager.ChangeTitle(args[2], author);

                    embed.WithDescription($"{author.Username} changed his lobby's title to: {target.GameTitle}");
                    embed.AddInlineField("In-lobby players:", target.Players);
                    embed.WithFooter($"{target.CurrentPlayers} / {target.PlayerCapacity} users", "https://i.imgur.com/9Y0vuCO.png");
                }
            }
            await Context.Channel.SendMessageAsync("", false, embed);
        }


        //! Games:

        [Command("card")]
        [Remarks("s:card [CardValue] (min 2)")]
        [Summary("User is provided a scratch card, of which they bet pebbles, winnable jackpot.")]
        public async Task ScratchCard(int cardValue)
        {//! Scratch ticket game
            if (cardValue < 2)
            {
                await Context.Channel.SendMessageAsync("Minimum scratch card price is 2 pebbles!");
            }
            else
            {
                Console.WriteLine("Config JP before adding card: " + Config.bot.jackpot);
                Random rnd = new Random();
                //! Potential items: Rainbow, Card, Strawberry, Heart, Gold Bar, Grape, Pineapple, Elephant, Crown. (9 things)
                int _slotOne = rnd.Next(0, 5);
                int _slotTwo = rnd.Next(0, 5);
                int _slotThree = rnd.Next(0, 5);
                string SlotOne = Global.scratchItems[_slotOne];
                string SlotTwo = Global.scratchItems[_slotTwo];
                string SlotThree = Global.scratchItems[_slotThree];

                SocketUser target = null;
                var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
                target = mentionedUser ?? Context.User;

                var account = UserAccounts.GetAccount(target);

                if (SlotOne == SlotTwo && SlotOne == SlotThree)
                {//! All three slots are the same.
                    string title = $"{account.UserName}'s Scratch Card:";
                    string message = $"{SlotOne} | {SlotTwo} | {SlotThree}\n" +
                        $"All of these items match! You win the Jackpot of {Config.bot.jackpot}!";

                    account.PEBBLES += Convert.ToInt32(Config.bot.jackpot);
                    Config.bot.jackpot = "0";
                    Config.UpdateConfig();

                    var embed = new EmbedBuilder();
                    embed.WithTitle(title);
                    embed.WithDescription(message);
                    embed.WithFooter("", "https://i.imgur.com/9Y0vuCO.png");
                    embed.WithColor(new Color(0, 255, 0));
                    await Context.Channel.SendMessageAsync("", false, embed);
                }
                else if (SlotOne == SlotTwo || SlotTwo == SlotThree || SlotOne == SlotThree)
                {//! Two Scratch positions match.
                    string title = $"{account.UserName}'s Scratch Card:";
                    string message = $"{SlotOne} | {SlotTwo} | {SlotThree}\n" +
                        $"Two of these items match! You get twice the card value back!";

                    account.PEBBLES += cardValue * 2;
                    int tempJack = Convert.ToInt32(Config.bot.jackpot);
                    tempJack += 25;
                    Config.bot.jackpot = Convert.ToString(tempJack);

                    int jackpot = Convert.ToInt32(Config.bot.jackpot) + cardValue; //! Get current jackpot and raise the value of it by price of card.
                    Config.bot.jackpot = Convert.ToString(jackpot); //! Update config's jackpot value with new value.

                    Config.UpdateConfig();

                    var embed = new EmbedBuilder();
                    embed.WithTitle(title);
                    embed.WithFooter("", "https://i.imgur.com/9Y0vuCO.png");
                    embed.WithDescription(message);
                    embed.WithColor(new Color(0, 255, 0));
                    await Context.Channel.SendMessageAsync("", false, embed);
                }
                else
                {//! No matching items
                    string title = $"{account.UserName}'s Scratch Card:";
                    string message = $"{SlotOne} | {SlotTwo} | {SlotThree}\n" +
                        $"None of these items match! You're out {cardValue} pebbles!";

                    account.PEBBLES -= cardValue; //! Reduce user's current balance.

                    int tempJack = Convert.ToInt32(Config.bot.jackpot);
                    tempJack += 25;
                    Config.bot.jackpot = Convert.ToString(tempJack);

                    int jackpot = Convert.ToInt32(Config.bot.jackpot) + cardValue; //! Get current jackpot and raise the value of it by price of card.
                    Config.bot.jackpot = Convert.ToString(jackpot); //! Update config's jackpot value with new value.

                    Config.UpdateConfig();

                    var embed = new EmbedBuilder();
                    embed.WithTitle(title);
                    embed.WithFooter("", "https://i.imgur.com/9Y0vuCO.png");
                    embed.WithDescription(message);
                    embed.WithColor(new Color(0, 255, 0));
                    await Context.Channel.SendMessageAsync("", false, embed);
                }
            }
        }

        [Command("coin")]
        [Remarks("s:coin [heads/tails] [bet amount]")]
        [Summary("Classic coin flip betting!")]
        public async Task CoinFlip(string choice, int betAmount)
        {
            //! Needed variabls:
            string result;
            Random rng = new Random();
            SocketUser user = Context.Message.Author;
            int userBalance = UserAccounts.GetAccount(user).PEBBLES;

            if (!(betAmount <= userBalance))
            {
                await Context.Channel.SendMessageAsync("Your bet couldn't go through, you're betting too much!");
            }
            else if (betAmount <= 0)
            {
                await Context.Channel.SendMessageAsync("Your bet couldn't go through, you're betting too little!\n" +
                    "Minimum bet: 1 pebble.");
            }
            else
            {
                int coin = rng.Next(0, 100);
                if (coin < 0 || coin > 100)
                    await Context.Channel.SendMessageAsync("Coin failed to flip, try again...", false);
                else
                {
                    if (coin >= 0 && coin <= 50)
                    {//! Heads
                        result = "heads";
                        if (choice.ToLower() == result || choice.ToLower() == "h")
                        {//! User chose heads, and won.
                            int winValue = betAmount * 2;
                            await Context.Channel.SendMessageAsync($"Coin flip won! You won {winValue} back!");
                            UserAccounts.UpdatePebbles(user, winValue);
                        }
                        else
                        {//! User chose tails, and lost.
                            await Context.Channel.SendMessageAsync($"Coin flip lost! You lost {betAmount}!");
                            userBalance -= betAmount;
                            UserAccounts.UpdatePebbles(user, -betAmount);
                        }
                    }
                    else if (coin > 50 && coin <= 100)
                    {//! Tails
                        result = "tails";
                        if (choice.ToLower() == result || choice.ToLower() == "t")
                        {//! User chose tails, and won.
                            int winValue = betAmount * 2;
                            await Context.Channel.SendMessageAsync($"Coin flip won! You won {winValue} back!");
                            UserAccounts.UpdatePebbles(user, winValue);
                        }
                        else
                        {//! User chose heads, and lost.
                            await Context.Channel.SendMessageAsync($"Coin flip lost! You lost {betAmount}!");
                            userBalance -= betAmount;
                            UserAccounts.UpdatePebbles(user, -betAmount);
                        }
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Coin flip failed!");
                    }
                }
            }
        }

        //! Commands:

        //[Command("WC")]
        //public async Task WCard()
        //{
        //    string css = "<style>\nh1\n{\n	color: rgb(0,255,0);\n	font-size: 80px;\n	text-align: center;\n	text-overflow: visible;\n}\n.userImg\n{\n  	position: relative;\n  \n  	top: 50px;\n  	left: 30%;\n\n	display: block;\n    width: 200px;\n    height: 200px;\n}\nbody\n{\n	background-image: url(\"https://i.pinimg.com/originals/3d/d2/e6/3dd2e617c44a15b516d2e83f2119bcbf.png\");\n	width: 500px;\n	height: 500px;\n}\n</style>";
        //    string html = String.Format("<h1>\n	Welcome {0}\n  		<img class=\"userImg\" src=\"{1}\"/>\n</h1>", Context.User.Username, Context.User.GetAvatarUrl());
        //    var converter = new HtmlToImageConverter
        //    {
        //        Width = 400,
        //        Height = 500
        //    };
        //    var jpgBytes = converter.GenerateImage(css + html, NReco.ImageGenerator.ImageFormat.Jpeg);
        //    await Context.Channel.SendFileAsync(new MemoryStream(jpgBytes), "WelcomeUser.Jpg");
        //}

        [Command("jackpot")]
        [Alias("jp")]
        [Remarks("s:jackpot")]
        [Summary("Returns the current value of the jackpot.")]
        public async Task JackpotValue()
        {
            await Context.Channel.SendMessageAsync($"Current jackpot value is: {Config.bot.jackpot}");
        }

        [Command("rps")]
        [Remarks("s:rps [r/p/s] [bet amount]")]
        [Summary("Play rock paper scissors!")]
        public async Task RockPaperScissors(char choice, int betAmount)
        {
            //! Needed variabls:
            char rps;
            Random rng = new Random();
            SocketUser user = Context.Message.Author;
            int userBalance = UserAccounts.GetAccount(user).PEBBLES;

            var embed = new EmbedBuilder();
            embed.WithTitle("RPS");
            embed.WithFooter("", "https://i.imgur.com/9Y0vuCO.png");
            embed.WithColor(new Color(0, 255, 0));
            Console.WriteLine("Runnign RPS");
            if (!(betAmount <= userBalance))
            {
                await Context.Channel.SendMessageAsync("Your bet couldn't go through, you're betting too much!");
            }
            else if (betAmount <= 0)
            {
                await Context.Channel.SendMessageAsync("Your bet couldn't go through, you're betting too little!\n" +
                    "Minimum bet: 1 pebble.");
            }
            else
            {
                switch (rng.Next(1,4))
                {
                    case 1:
                        rps = 'r';
                        break;
                    case 2:
                        rps = 'p';
                        break;
                    case 3:
                        rps = 's';
                        break;

                    default:
                        rps = 'r';
                        break;
                }

                if(choice != rps)
                {
                    //! Player winnings
                    if(choice == 'r' && rps != 'p')
                    { //! Player wins R vs S
                        embed.WithDescription($"{user.Username} wins! Money gained: {betAmount * 2}");
                        UserAccounts.UpdatePebbles(user, betAmount * 2);
                    }
                    else if(choice == 'p' && rps != 's')
                    { //! Player wins P vs R
                        embed.WithDescription($"{user.Username} wins! Money gained: {betAmount * 2}");
                        UserAccounts.UpdatePebbles(user, betAmount * 2);
                    }
                    else if(choice == 's' && rps != 'r')
                    { //! Player wins S vs P
                        embed.WithDescription($"{user.Username} wins! Money gained: {betAmount * 2}");
                        UserAccounts.UpdatePebbles(user, betAmount * 2);
                    } 
                    //! Slab winnings:
                    else if (choice == 'r' && rps == 'p')
                    { //! Slab wins P vs R
                        embed.WithDescription($"Slab wins! Money lost: {betAmount}");
                        UserAccounts.UpdatePebbles(user, -betAmount);
                    }
                    else if (choice == 'p' && rps == 's')
                    { //! Slab wins S vs P
                        embed.WithDescription($"Slab wins! Money lost: {betAmount}");
                        UserAccounts.UpdatePebbles(user, -betAmount);
                    }
                    else if (choice == 's' && rps == 'r')
                    { //! Slab wins R vs S
                        embed.WithDescription($"Slab wins! Money lost: {betAmount}");
                        UserAccounts.UpdatePebbles(user, -betAmount);
                    }
                    await Context.Channel.SendMessageAsync("", false, embed);
                }
                else
                {
                    embed.WithDescription("Tie! No wins or losses today!");
                    await Context.Channel.SendMessageAsync("", false, embed);
                }
            }
        }

        [Command("WorldDomination")]
        [Alias("wd")]
        [Remarks("s:WorldDomination")]
        [Summary("Rule the world!")]
        public async Task WorldDomination([Remainder] string arg = "")
        {
            Color red = new Color(255, 0, 0);
            Color green = new Color(0, 255, 0);
            Color blue = new Color(0, 0, 255);

            int playerCount;
            List<SocketUser> players;
            List<WorldAccount> WDplayers = new List<WorldAccount>();

            string context;

            string []args = arg.Split(' '); //! Seperate multiple args

            if (!Global.WDRunning) //! Can't have any double instances.
            {
                playerCount = 0;
                players = new List<SocketUser>
                {
                    Capacity = 5
                };
                Global.WDRunning = true;
                Console.WriteLine("World domination beginning...");
                context = "";

                players.Add(Context.Message.Author); //! Make sure the game starter is added!

                foreach (SocketUser user in Context.Message.MentionedUsers)
                {
                    players.Add(user);
                }
                playerCount = players.Count();

                foreach (SocketUser u in players)
                {
                    WDplayers.Add(WorldStorage.GetAccount(u));
                    WorldStorage.UpdateInfo(u);
                }
                Console.WriteLine($"{players.Count()} = {WDplayers.Count()}");

                Console.WriteLine("Players:");
                foreach (SocketUser player in players)
                {
                    Console.Write($"{ UserAccounts.GetAccount(player).UserName}, ");
                }

                Global.players = players;
                Global.playerCount = playerCount;

                await UpdateGameEmbed(green,"Game starting!");
            }
            else if (Global.WDRunning && args[0] == "close")
            {
                Global.WDRunning = false;
                Global.playerCount = 0;
                Global.players.Clear();
                context = "";
                await UpdateGameEmbed(red, "Game closed.");
                return;
            }
            else if (Global.WDRunning && args[0] == "join" && Global.playerCount < Global.players.Capacity)
            {
                Global.players.Add(Context.Message.Author);
                Global.playerCount = Global.players.Count();
                await UpdateGameEmbed(green, $"{Context.Message.Author.Username} has  joined!");
            }
            else if(Global.WDRunning && args[0] == "action")
            {
                //! If statements for actions using args[1] and so on. If targeted, use context.message.mentionedUsers for target.
                if(args[1] == "fight")
                {
                    string fighter1, fighter2;
                    fighter1 = Context.Message.Author.Username;
                    fighter2 = Context.Message.MentionedUsers.First<SocketUser>().Username; //! Broken but point is visable
                    var embed = new EmbedBuilder();
                    embed.WithColor(green);
                    embed.WithTitle("World Domination");                    
                    embed.AddField("Fight between", $"{fighter1} and {fighter2}");
                    embed.AddInlineField($"{fighter1} info", "Info on author's tribe");
                    embed.AddInlineField($"{fighter2} info", "Info on mentioned user's tribe");
                    embed.WithFooter($"{Global.playerCount}/{Global.players.Capacity} players", "https://i.imgur.com/9Y0vuCO.png");

                    await Context.Channel.SendMessageAsync("", false, embed);
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("Game already running!");
            }
        }

        private async Task UpdateGameEmbed(Color colour, string content = "")
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("World Domination");
            embed.WithFooter($"{Global.playerCount}/{Global.players.Capacity} players", "https://i.imgur.com/9Y0vuCO.png");
            embed.WithColor(colour);
            embed.WithDescription(content);
            await Context.Channel.SendMessageAsync("", false, embed);
        }

        //! General Commands
        [Command("Help")]
        [Remarks("s:Help")]
        [Summary("Help center.")]
        public async Task Help()
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("Help/Command List:");
            embed.WithFooter("", "https://i.imgur.com/9Y0vuCO.png");
            embed.WithDescription("" +
                "Info    - Provides a description about Slab Bot.\n" +
                "LVL     - Provides the user info on their saved level and rank.\n" +
                "Purge   - Removes given amount of messages from current channel.\n" +
                "Help   - Provides this menu.\n" +
                "HelpG  - Provides game commands.\n" +
                "LVL    - Shows mentioned user's level (defaults to OP).\n" +
                "BAL    - Returns mentioned user's current balance (defaults to OP).");
            embed.WithColor(new Color(0, 255, 0));

            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("HelpG")]
        [Remarks("s:HelpG")]
        [Summary("Game help center.")]
        public async Task HelpGames()
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("Games help List:");
            embed.WithFooter("", "https://i.imgur.com/9Y0vuCO.png");
            embed.WithDescription("" +
                "coin   - Flip a coin and bet on the side! (Min bet 1)\n" +
                "card   - Buy a scratch card for a chance to win the jackpot! (Min bet 2)\n" +
                "Jackpot    - Returns the current value of the jackpot.\n" +
                "rps    - Play Rock Paper Scissors against Slab! (Min bet 1)");
            embed.WithColor(new Color(0, 255, 0));

            await Context.Channel.SendMessageAsync("", false, embed);
        }


        //[Command("Echo")]
        //[Remarks("s:Echo [Message]")]
        //[Summary("Gives you your message back.")]
        //public async Task Echo([Remainder]string message)
        //{
        //    var embed = new EmbedBuilder();
        //    embed.WithTitle("Echoed message:");
        //    embed.WithDescription(message);
        //    embed.WithColor(new Color(0, 255, 0));

        //    await Context.Channel.SendMessageAsync("", false, embed);
        //}

        //[Command("Hello")]
        //[Remarks("s:Hello")]
        //[Summary("Slab says hello to you.")]
        //public async Task Hello()
        //{
        //    await Context.Channel.SendMessageAsync("Hello " + Context.User.Username + "!");
        //}

        //[Command("Slab")
        //[Remarks("s:Slab")]
        //[Summary("Slab sends you a picture of his family.")]
        //public async Task Slab()
        //{
        //! Removed 2019-11-19 for return of Slab in v1.2
        //! Reason: uncessary meme
        //    Random rng = new Random();
        //    int slabId = rng.Next(1, 15);

        //    await Context.Channel.SendFileAsync($"SlabImage/Slab{slabId}.jpg", "Here is your slab!");
        //}

        [Command("Info")]
        [Remarks("s:Info")]
        [Summary("Provides you with basic info on the bot.")]
        public async Task Info()
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("Slab Info:");
            embed.WithFooter("", "https://i.imgur.com/9Y0vuCO.png");
            embed.WithDescription("Slab Bot was made by Concrete#1761 for fun!");
            embed.WithColor(new Color(0, 255, 0));

            await Context.Channel.SendMessageAsync("", false, embed);
        }

        //[Command("GT")]
        //[Remarks("s:GT [Your future green text here]")]
        //[Summary("Not a car, but a green text formatter. USE \">\" TO START NEW LINE.")]
        //public async Task GreenText([Remainder]string message)
        //{

            //! Removed 2019-11-19
            //! Reason: dead meme
        //    string[] lines = message.Split(new char[] { '>' }, StringSplitOptions.RemoveEmptyEntries);

        //    char chev = '>';
        //    string content = "";
        //    foreach (string line in lines)
        //    {
        //        content += chev + line + "\n";
        //    }

        //    string codePrefix = "```css\n";
        //    string codeSuffix = "```";

        //    string greenText = codePrefix + content + codeSuffix;

        //    await Context.Channel.SendMessageAsync(greenText);
        //}

        [Command("LVL")]
        [Remarks("s:Lvl [Mentioned User (Optional)]")]
        [Summary("Shows the stats of the pinged user (default is your stats).")]
        public async Task GetLVL([Remainder] string arg = "")
        {
            SocketUser target = null;
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
            target = mentionedUser ?? Context.User;

            var account = UserAccounts.GetAccount(target);

            string rank = UserAccounts.assignRank(account);

            string message = $"Level: {account.LVL}\n" +
                $"{target.Username}'s XP is: {account.XP}/ {account.LVL * 500}\n" +
                $"and has a balance of {account.PEBBLES}";
            string title = $"Rank: {rank}";

            var embed = new EmbedBuilder();
            embed.WithTitle(title);
            embed.WithFooter("", "https://i.imgur.com/9Y0vuCO.png");
            embed.WithDescription(message);
            embed.WithColor(new Color(0, 255, 0));
            embed.ThumbnailUrl = target.GetAvatarUrl().ToString();
            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("BAL")]
        [Remarks("s:BAL")]
        [Summary("Provides the user with their current balance.")]
        public async Task GetPebbles([Remainder] string arg = "")
        {
            SocketUser target = null;
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
            target = mentionedUser ?? Context.User;

            var account = UserAccounts.GetAccount(target);

            await Context.Channel.SendMessageAsync($"Your current pebble count is: {account.PEBBLES}.");
        }

        //[Command("ID")]
        //[Remarks("s:Id [Mentioned User (Optional)]")]
        //[Summary("Provides you the targeted user's unique ID, default is yourself, in a DM.")]
        //[RequireUserPermission(GuildPermission.Administrator)]
        //public async Task GetID([Remainder] string arg = "")
        //{
        //    SocketUser target = null;
        //    var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
        //    target = mentionedUser ?? Context.User;

        //    var account = UserAccounts.GetAccount(target);
        //    var dmChannel = await Context.User.GetOrCreateDMChannelAsync();

        //    await dmChannel.SendMessageAsync($"{target.Username}'s unique ID is: {account.ID}");
        //}

        //[Command("Kick")]
        //[Remarks("s:Kick [Mentioned User] [Reason (Optional)]")]
        //[Summary("Kicks the mentioned user and sends a \"You have been kicked\" message.")]
        //[RequireUserPermission(GuildPermission.KickMembers)]
        //[RequireBotPermission(GuildPermission.KickMembers)]
        //public async Task KickUser(SocketGuildUser user, string reason = "No reason provided.")
        //{
        //    await user.KickAsync(reason);
        //}

        //[Command("Ban")]
        //[Remarks("s:Ban [Mentioned User] [Reason (Optional)]")]
        //[Summary("Bans the mentioned user and sends a \"You have been Banned\" message.")]
        //[RequireUserPermission(GuildPermission.BanMembers)]
        //[RequireBotPermission(GuildPermission.BanMembers)]
        //public async Task BanUser(SocketGuildUser user, int pruneLength = 5, string reason = "No reason provided.")
        //{
        //    await user.Guild.AddBanAsync(user, pruneLength, reason);
        //}

        //[Command("Warn")]
        //[Remarks("s:Warn [Mentioned User] [Reason (Optional)]")]
        //[Summary("Warns the mentioned user and sends a \"You have been Warned\" message.")]
        //[RequireUserPermission(GuildPermission.Administrator)]
        //[RequireBotPermission(GuildPermission.Administrator)]
        //public async Task WarnUser(SocketGuildUser user, string reason = "No reason provided")
        //{
        //    var userAccount = UserAccounts.GetAccount(user);
        //    userAccount.NumberOfWarnings++;
        //    UserAccounts.SaveAccounts();

        //    //! Warn count check:
        //    if (userAccount.NumberOfWarnings >= 3)
        //    {
        //        await KickUser(user, reason);
        //    }
        //}

        [Command("Purge")]
        [Remarks("s:Purge [Amount to Delete]")]
        [Summary("Deletes last x amount of messages")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task Purge(uint purgeCount = 10)
        {
            var messages = await this.Context.Channel.GetMessagesAsync((int)purgeCount + 1).Flatten();

            await this.Context.Channel.DeleteMessagesAsync(messages);
            const int delay = 5000;
            var m = await this.ReplyAsync($"Purge completed. _This message will be deleted in {delay / 1000} seconds._");
            await Task.Delay(delay);
            await m.DeleteAsync();
        }

        //! Owner only commands:

        //[Command("BC")]
        //[Remarks("s:BC [Message]")]
        //[Summary("Broadcasts to every server Slab is in.")]
        //[RequireOwner]
        //public async Task BC([Remainder]string message)
        //{
        //    var embed = new EmbedBuilder();
        //    embed.WithTitle("BROADCAST");
        //    embed.WithDescription(message);
        //    embed.WithColor(new Color(255, 0, 0));
        //    foreach (SocketGuild guild in Global.Client.Guilds)
        //    
        //        foreach (SocketTextChannel textChannel in guild.TextChannels)
        //        {
        //            if (textChannel.Name.ToLower() == "#general")
        //            {
        //                await Context.Channel.SendMessageAsync("", false, embed);
        //                break;
        //            }
        //        }
        //    }
        //}
    }
}