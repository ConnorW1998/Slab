using Newtonsoft.Json;
using Slab.Core.UserAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Slab.Modules.WDfiles;

namespace Slab.Core
{
    public static class DataStorage
    {
        //! Save all userAccounts
        public static void SaveUserAccounts(IEnumerable<UserAccount> accounts, string filePath)
        {
            string json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        //! Get all userAccounts
        public static IEnumerable<UserAccount> LoadUserAccounts(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<UserAccount>>(json);
        }

        //! Save all WorldAccounts
        public static void SaveWorldAccounts(IEnumerable<WorldAccount> accounts, string filePath)
        {
            string json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        //! Get all WorldAccounts
        public static IEnumerable<WorldAccount> LoadWorldAccounts(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<WorldAccount>>(json);
        }

        //! Save all GamePrefs
        public static void SaveGamePrefs(GamePref preferences, string filePath)
        {
            string json = JsonConvert.SerializeObject(preferences, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        //! Get all GamePrefs
        public static GamePref LoadGamePrefs(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<GamePref>(json);
        }

        public static bool SaveExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}