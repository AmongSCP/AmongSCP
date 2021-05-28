﻿using System;
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

        public Player[] PickPlayers()
        {
            var count = Math.Min(_players.Count, 8);
            
            var output = _players.Take(count);
            _players.RemoveRange(0, count);

            return output.ToArray();
        }
    }
}