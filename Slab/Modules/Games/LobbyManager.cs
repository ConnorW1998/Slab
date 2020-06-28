using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Slab.Modules.Games
{
    public class LobbyManager
    {
        static List<Lobby> Lobbies;

        public static void AddLobby(SocketUser su, string gameTitle, List<SocketUser> forcedPlayers = null)
        {
            Lobby lobby = new Lobby
            {
                Owner = su,
                GameTitle = gameTitle,
                CurrentPlayers = 0,
                PlayerCapacity = 10,
                Players = new List<SocketUser>()
            };

            lobby.Players.Add(su);

            if (forcedPlayers.Count >= 1)
            {
                lobby.Players.AddRange(forcedPlayers);
            }

            Lobbies.Add(lobby);
        }

        public static void RemoveLobby(Lobby targetLobby)
        {
            Lobbies.Remove(targetLobby);
        }

        public static void SetCapacity(SocketUser user, int newCapacity)
        {
            foreach (Lobby l in Lobbies)
            {
                if (l.Owner.Id == user.Id)
                {
                    l.PlayerCapacity = newCapacity;
                }
            }
        }

        public static void AddPlayer(Lobby targetLobby, SocketUser newPlayer)
        {
            if(targetLobby.CurrentPlayers < targetLobby.PlayerCapacity)
            {
                targetLobby.Players.Add(newPlayer);
                targetLobby.CurrentPlayers = targetLobby.Players.Count();
            }
            return;
        }

        public static void RemovePlayer(Lobby targetLobby, SocketUser oldPlayer)
        {
            if (targetLobby != FindLobby(oldPlayer))
            {
                targetLobby = FindLobby(oldPlayer);
            }

            if(targetLobby.Owner == oldPlayer)
            {
                RemoveLobby(targetLobby);
            }

            targetLobby.Players.Remove(oldPlayer);
            targetLobby.CurrentPlayers = targetLobby.Players.Count();
        }

        public static void ChangeTitle(string newTitle, SocketUser user)
        {
            Lobby target = FindLobby(user);
            if(target.Owner.Id == user.Id)
            {
                target.GameTitle = newTitle;
            }
        }

        public static Lobby FindLobby(SocketUser user)
        {
            foreach (Lobby l in Lobbies)
            {
                if (l.Players.Contains(user))
                {
                    return l;
                }
            }
            return null;
        }

        public static Lobby FindLobby(string lobbyName)
        {
            foreach(Lobby l in Lobbies)
            {
                if(l.GameTitle.ToLower() == lobbyName.ToLower())
                {
                    return l;
                }
            }
            return null;
        }
    }

    public class Lobby
    {
        public string GameTitle;
        public SocketUser Owner;
        public List<SocketUser> Players;
        public int CurrentPlayers;
        public int PlayerCapacity;

        public Lobby()
        {
            GameTitle = null;
            Owner = null;
            Players = new List<SocketUser>();
            CurrentPlayers = 0;
            PlayerCapacity = 0;
        }
    }
}
