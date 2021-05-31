using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using mattmc3.dotmore.Collections.Generic;
using MEC;

namespace AmongSCP.PlayerManager
{
    public class PlayerManager
    {
        public readonly List<Player> Imposters = new List<Player>();
        
        public readonly List<Player> Crewmates = new List<Player>();
        
        public readonly List<Player> AlivePlayers = new List<Player>();

        public readonly OrderedDictionary<Player, PlayerInfo> Players = new OrderedDictionary<Player, PlayerInfo>();

        public void EndGame()
        {
            Imposters.Clear();
            Crewmates.Clear();
            AlivePlayers.Clear();
            Players.Clear();
        }

        public void ReloadLists()
        {
            Imposters.Clear();
            Crewmates.Clear();
            AlivePlayers.Clear();
            
            Imposters.AddRange(Players.Where(pair => pair.Value.Role == Role.Imposter).Select(pair => pair.Key));
            Crewmates.AddRange(Players.Where(pair => pair.Value.Role == Role.Crewmate).Select(pair => pair.Key));
            AlivePlayers.AddRange(Players.Where(pair => pair.Value.Role != Role.None && pair.Value.IsAlive).Select(pair => pair.Key));
        }
        
        public void UpdateQueue()
        {
            Timing.CallDelayed(.1f, UpdateQueueNoWait);
        }

        public void UpdateQueueNoWait()
        {
            foreach (var player in Players.Keys.ToList())
            {
                if (player == null || Player.List.Contains(player)) continue;

                Players.Remove(player);
            }
            
            foreach (var player in Player.List)
            {
                if (Players.ContainsKey(player)) continue;
                    
                Players[player] = new PlayerInfo(this);
            }
            
            ReloadLists();
        }

        public Player[] PickPlayers(int num)
        {
            var count = Math.Min(Players.Count, num);
            
            var output = Players.Take(count).ToArray();

            for (var i = 0; i < count; i++)
            {
                Players.RemoveAt(0);
            }

            return output.Select(pair => pair.Key).ToArray();
        }

        public void ClearQueued()
        {
            foreach (var pair in Players)
            {
                pair.Key.Role = RoleType.Spectator;
            }
        }
        
        public void ClearImposters()
        {
            foreach (var player in Imposters)
            {
                player.Role = RoleType.Spectator;
            }
        }
        
        public void ClearCrewmates()
        {
            foreach (var player in Crewmates)
            {
                player.Role = RoleType.Spectator;
            }
        }
    }
}