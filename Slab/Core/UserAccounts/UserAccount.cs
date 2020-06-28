using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slab.Core.UserAccounts
{
    public class UserAccount
    {
        public ulong ID { get; set; }
        public string UserName { get; set; }
        public int XP { get; set; }
        public int LVL { get; set; }
        public int PEBBLES { get; set; }
        public bool isMuted { get; set; }
        public uint NumberOfWarnings { get; set; }
        public Dictionary<ulong, DateTime> DictionaryDates { get; set; }
    }
}
