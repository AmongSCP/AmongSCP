using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace AmongSCP
{
    public class PlayerManager
    {
        private readonly List<Player> _players = new List<Player>();

        private TaskManager _taskManager;
        
        public readonly List<Player> Imposters = new List<Player>();
        
        public readonly List<Player> Crewmates = new List<Player>();
        
        public readonly List<Player> DeadPlayers = new List<Player>();
        
        public readonly List<Vector3> DeadPositions = new List<Vector3>();

        public readonly List<Player> CalledEmergencyMeeting = new List<Player>();

        public readonly Dictionary<Player, DateTime> LastShot = new Dictionary<Player, DateTime>();

        public PlayerManager(TaskManager taskManager)
        {
            _taskManager = taskManager;
        }

        public void EndGame()
        {
            Imposters.Clear();
            Crewmates.Clear();
            DeadPlayers.Clear();
            DeadPositions.Clear();
            CalledEmergencyMeeting.Clear();
            LastShot.Clear();
        }
        
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

            UpdateList(_players);
            UpdateList(Imposters);
            UpdateList(Crewmates);
            UpdateList(CalledEmergencyMeeting);
        }

        private void UpdateList(ICollection<Player> list)
        {
            foreach (var player in list.ToList())
            {
                if (player != null && Player.List.Contains(player)) continue;

                list.Remove(player);
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