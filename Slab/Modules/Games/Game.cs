using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Slab.Modules.Games
{
    public static class Game
    {
        static Game()
        {
            string Title = null;
            int RoundLimit = 0;
            int PlayerLimit = 0;
            int PlayerCount = 0;
            List<Player> Players = new List<Player>();
        }
    }

    public class Player
    {
        static Player()
        {
            ulong ID = 0;
            int HP = 0;

            string username = null;
            string role = null;

            List<Unit> units = new List<Unit>();
        }
    }

    public class Unit
    {
        public string Class;
        public string Owner;
        public int HP;
        public int locX;
        public int locY;

        public Unit()
        {
            Class = null;
            Owner = null;
            HP = 0;
            locX = 0; locY = 0;
        }

        public Unit(string className = null, string unitOwner = null, int classHealth = 0, int posX = 0, int posY = 0)
        {
            Class = className;
            Owner = unitOwner;
            HP = classHealth;
            locX = posX; locY = posY;
        }
    }
}
