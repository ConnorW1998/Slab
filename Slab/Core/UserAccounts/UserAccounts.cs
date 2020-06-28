using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slab.Core.UserAccounts
{
    public static class UserAccounts
    {
        public static SocketUser su;
        public static String[] roles = new String[10];
        private static List<UserAccount> accounts;
        private static string accountsFile = "Resources/accounts.json";
        static UserAccounts()
        {
            if (DataStorage.SaveExists(accountsFile))
            {
                accounts = DataStorage.LoadUserAccounts(accountsFile).ToList();
            }
            else
            {
                accounts = new List<UserAccount>();
                SaveAccounts();
            }
        }

        public static void SaveAccounts()
        {
            DataStorage.SaveUserAccounts(accounts, accountsFile);
        }


        //We paste the AccountExit shit here
        public static bool AccountExists(SocketUser user)
        {
            ulong id = user.Id;
            IEnumerable<UserAccount> result = from a in accounts where a.ID == id select a;
            UserAccount foundAccount = result.FirstOrDefault();
            if (foundAccount == null) return false;
            return true;
        }


        public static UserAccount GetAccount(SocketUser user)
        {
            su = user;
            return ValidateAccount(user.Id);
        }

        private static UserAccount ValidateAccount(ulong id)
        {
            var result = from a in accounts
                         where a.ID == id
                         select a;

            var account = result.FirstOrDefault();

            if (account == null)
                account = CreateUserAccount(id);

            return account;
        }

        private static UserAccount CreateUserAccount(ulong id)
        {
            var newAccount = new UserAccount()
            {
                ID = id,
                UserName = "",
                XP = 0,
                LVL = 1,
                PEBBLES = 10,
                isMuted = false,
                DictionaryDates = new Dictionary<ulong, DateTime> { { ((SocketGuildUser)su).Guild.Id, DateTime.Now } }
            };

            accounts.Add(newAccount);
            SaveAccounts();
            return newAccount;
        }

        public static void updateInfo(SocketUser user)
        {
            var account = ValidateAccount(user.Id);
            account.UserName = user.Username;
            if (account.XP >= account.LVL * 500) //! Increased XP gap from 50 -> 500 in version 1.2
            {
                assignRank(account);
                account.XP = 0;
                account.LVL += 1;
                account.PEBBLES += 100;
            }
            var guildUser = (SocketGuildUser)user;
            Dictionary<ulong, DateTime> DictionaryDates = new Dictionary<ulong, DateTime>();
            account.DictionaryDates = DictionaryDates;

            SaveAccounts();
        }
        public static void UpdatePebbles(SocketUser user, int pebbleAmount)
        {
            var account = ValidateAccount(user.Id);
            account.PEBBLES += pebbleAmount;
            SaveAccounts();
        }

        public static string assignRank(UserAccount account)
        {
            if (account.LVL == 1)
                return "Slab of Cake";
            else if (account.LVL == 2)
                return "Slab of Cardboard";
            else if (account.LVL == 3)
                return "Slab of Plastic";
            else if (account.LVL == 4)
                return "Slab of Wood";
            else if (account.LVL == 5)
                return "Slab of Clay";
            else if (account.LVL == 6)
                return "Slab of Stone";
            else if (account.LVL == 7)
                return "Slab of Concrete";
            else if (account.LVL == 8)
                return "Slab of Granite";
            else if (account.LVL == 9)
                return "Slab of Marble";
            else if (account.LVL == 10)
                return "Slab of Ebony";
            else
                return null;
        }
    }
}
