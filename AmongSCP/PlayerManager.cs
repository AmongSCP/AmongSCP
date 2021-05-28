using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;

namespace AmongSCP
{
    public class PlayerManager
    {
        private List<Player> _players = new List<Player>();
        
        public List<Player> Imposters = new List<Player>();
        public List<Player> Crewmates = new List<Player>();

        public void UpdateQueue()
        {
            Timing.CallDelayed(.1f, () =>
            {
                UpdateQueueNoWait();
            });
        }

        public void UpdateQueueNoWait()
        {
            foreach (var player in Player.List)
            {
                if (_players.Contains(player)) continue;
                    
                _players.Add(player);
            }

            foreach (var player in _players.ToList())
            {
                if (player != null && Player.List.Contains(player)) continue;

                _players.Remove(player);
            }
            
            foreach (var player in Imposters.ToList())
            {
                if (player != null && Player.List.Contains(player)) continue;

                Imposters.Remove(player);
            }
            
            foreach (var player in Crewmates.ToList())
            {
                if (player != null && Player.List.Contains(player)) continue;

                Crewmates.Remove(player);
            }
        }

        public Player[] PickPlayers(int num)
        {
            var count = Math.Min(_players.Count, num);
            
            var output = _players.Take(count).ToArray();
            _players.RemoveRange(0, count);

            return output;
        }

        public void ClearQueued()
        {
            foreach (var player in _players)
            {
                player.Role = RoleType.Spectator;
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