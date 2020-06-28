using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Slab.Core;
using Slab.Modules.Games;

namespace Slab.Modules.WDfiles
{
    public class WorldAccount
    {
        public ulong ID { get; set; }
        public string UserName { get; set; }
        public int XP { get; set; }
        public int LVL { get; set; }
        public int Gold { get; set; }
        public List<Unit> units { get; set; }
    }

   
    public class Cleric : Unit
    {
        public void SET()
        {
            
        }
    }

    public class Warrior : Unit
    {
        public void SET()
        {
            Class = "warrior";
            HP = 3;
        }
    }

    public class Scout : Unit
    {   
        public void SET()
        {
            Class = "scout";
            HP = 3;
        }
    }

    public class Base : Unit
    {
        public void SET()
        {
            Class = "base";
            HP = 10;
        }
    }

    public static class WorldStorage
    {
        public static SocketUser su;
        private static string WDFile = "Resources/WDinfo.json";
        private static List<WorldAccount> accounts;

        static WorldStorage()
        {
            if (DataStorage.SaveExists(WDFile))
            {
                accounts = DataStorage.LoadWorldAccounts(WDFile).ToList();
            }
            else
            {
                accounts = new List<WorldAccount>();
                SaveWorldAccounts();
            }
        }

        public static void SaveAccounts()
        {
            DataStorage.SaveWorldAccounts(accounts, WDFile);
        }

        public static void SaveWorldAccounts()
        {
            DataStorage.SaveWorldAccounts(accounts, WDFile);
        }
        
        public static bool AccountExists(SocketUser user)
        {
            ulong id = user.Id;
            IEnumerable<WorldAccount> result = from a in accounts where a.ID == id select a;
            WorldAccount foundAccount = result.FirstOrDefault();
            if (foundAccount == null) return false;
            return true;
        }

        public static WorldAccount GetAccount(SocketUser user)
        {
            su = user;
            return ValidateAccount(user.Id);
        }

        private static WorldAccount ValidateAccount(ulong id)
        {
            var result = from a in accounts
                         where a.ID == id
                         select a;

            var account = result.FirstOrDefault();

            if (account == null)
                account = CreateUserAccount(id);

            return account;
        }

        private static WorldAccount CreateUserAccount(ulong id)
        {
            var newAccount = new WorldAccount()
            {
                ID = id,
                UserName = "",
                XP = 0,
                LVL = 1,
                Gold = 0,
                units = new List<Unit>()
            };
            newAccount.units.Add(new Base());
            accounts.Add(newAccount);
            SaveAccounts();
            return newAccount;
        }

        public static void UpdateInfo(SocketUser user)
        {
            var account = ValidateAccount(user.Id);
            account.UserName = user.Username;

            foreach(Unit u in account.units)
            {
                //! Making sure the units know who their owner is
                u.Owner = account.UserName;
            }

            if (account.XP >= account.LVL * 100)
            {
                //! in-game leveling probably just going to increase gold for achieving it.
                account.Gold += 20;
                account.LVL += 1;
                account.XP = 0;
            }
            var guildUser = (SocketGuildUser)user;

            SaveAccounts();
        }

        public static void GameEnd()
        {
            foreach (WorldAccount wa in WorldStorage.accounts)
            {
                wa.units.Clear();
                wa.units.Add(new Base());
            }
        }
    }
    public class WDGame
    {
        public static string Fight(SocketUser fighter1, SocketUser fighter2)
        {
            WorldAccount a = WorldStorage.GetAccount(fighter1);
            WorldAccount b = WorldStorage.GetAccount(fighter2);

            //! Mapless fighting;
            Queue<Unit> warriorsA = new Queue<Unit>();
            Queue<Unit> warriorsB = new Queue<Unit>();

            foreach(Unit u in a.units)
            {
                if (u.Class == "warrior")
                {
                    warriorsA.Enqueue(u);
                }
            }

            foreach (Unit u in b.units)
            {
                if (u.Class == "warrior")
                {
                    warriorsB.Enqueue(u);
                }
            }

            if (warriorsA.Count() > 0 && warriorsB.Count() > 0)
            {
                //! Warriors fight warriors
                warriorsB.Peek().HP -= 1;
                if(warriorsB.Peek().HP <= 0)
                {
                    warriorsB.Dequeue(); //! Remove dead warrior

                    WorldStorage.UpdateInfo(fighter1);
                    WorldStorage.UpdateInfo(fighter2);

                    return $"{fighter1.Username} took out {fighter2.Username}'s warrior!";
                }

                WorldStorage.UpdateInfo(fighter1);
                WorldStorage.UpdateInfo(fighter2);

                return $"{fighter1.Username} attacked {fighter2.Username}'s warrior!";
            }
            else if (warriorsA.Count() > 0 && warriorsB.Count() == 0)
            {
                //! a warrior attack b base
                b.units[0].HP -= 1;

                WorldStorage.UpdateInfo(fighter1);
                WorldStorage.UpdateInfo(fighter2);

                return $"{fighter1.Username} attacks {fighter2.Username}'s base!";
            }
            else if (warriorsA.Count() == 0 && warriorsB.Count() > 0)
            {
                //! b warrior attack a base
                //! This case is because playerA tried attacking w/ no one to attack with.
                a.units[0].HP -= 1;

                WorldStorage.UpdateInfo(fighter1);
                WorldStorage.UpdateInfo(fighter2);

                return $"{fighter2.Username} attacks {fighter1.Username}'s base!";
            }
            //! Both have 0
            return "Neither player can attack, need to purchase warriors!";
        }

        public static bool Search(SocketUser user)
        {
            WorldAccount a = WorldStorage.GetAccount(user);

            Queue<Unit> scoutsA = new Queue<Unit>();

            foreach (Unit u in a.units)
            {
                if (u.Class == "warrior")
                {
                    scoutsA.Enqueue(u);
                }
            }

            if (scoutsA.Count() > 0)
            {
                Random random = new Random();
                if(random.Next(0, 100) >= 75)
                {
                    a.Gold += 5;
                }
                WorldStorage.UpdateInfo(user);
                return true;
            }
            return false;
        }

        public static string Buy(SocketUser user, string unit)
        {
            WorldAccount a = WorldStorage.GetAccount(user);

            if (unit == "cleric" && a.Gold >= 10)
            {
                a.units.Add(new Cleric());
                a.Gold -= 10;
                WorldStorage.UpdateInfo(user);
                return "Cleric unit purchased";
            }
            else if(unit == "warrior" && a.Gold >= 10)
            {
                a.units.Add(new Warrior());
                a.Gold -= 10;
                WorldStorage.UpdateInfo(user);
                return "Warrior unit purchased";
            }
            else if(unit == "scout" && a.Gold >= 10)
            {
                a.units.Add(new Scout());
                a.Gold -= 10;
                WorldStorage.UpdateInfo(user);
                return "Scout unit purchased";
            }

            return $"Not enough gold for {unit}. {a.Gold}/10 gold.";
        }
    }

    public class GameMap
    {

    }

    public class GamePref
    {
        public int[,] mapSize { get; set; }
        public string gameMode { get; set; }

        static GamePref()
        {
            
        }
    }

    public class GamePrefStorage
    {
        private static string GamePrefsFile = "Resources/GamePrefs.json";
        private static GamePref prefs;
        public static SocketUser su;

        private static int[,] defaultSize = new int[6, 6];
        static GamePrefStorage()
        {
            if (DataStorage.SaveExists(GamePrefsFile))
            {
                prefs = DataStorage.LoadGamePrefs(GamePrefsFile);
            }
            else
            {
                prefs = new GamePref();
            }
        }

        public static void SaveGameprefs()
        {
            DataStorage.SaveGamePrefs(prefs, GamePrefsFile);
        }

        public static GamePref GetPreferences()
        {
            return DataStorage.LoadGamePrefs(GamePrefsFile);
        }

        private static GamePref CreateUserAccount(ulong id)
        {
            var newAccount = new GamePref()
            {
                mapSize = new int[6,6],
                gameMode = "mapless"
            };
            return newAccount;
        }

        public static void UpdateInfo(int[,] mapSize = null, string gameMode = "mapless")
        {
            //! Storing incase ref:  mapSize = new int[,] { { 6, 6 } };
            GamePref preferences = GetPreferences();

            if (preferences.mapSize != mapSize)
            {
                preferences.mapSize = mapSize;
            }

            if (preferences.gameMode != gameMode)
            {
                preferences.gameMode = gameMode;
            }

            SaveGameprefs();
        }
    }
}
