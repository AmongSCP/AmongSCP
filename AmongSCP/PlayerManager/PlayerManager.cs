﻿using System;
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

        public readonly List<Player> DeadPlayers = new List<Player>();

        public readonly OrderedDictionary<Player, PlayerInfo> Players = new OrderedDictionary<Player, PlayerInfo>();

        public void EndGame()   
        {
            Imposters.Clear();
            Crewmates.Clear();
            AlivePlayers.Clear();
            DeadPlayers.Clear();
        }
        
        public void ReloadLists()
        {
            Imposters.Clear();
            Crewmates.Clear();
            AlivePlayers.Clear();
            DeadPlayers.Clear();
            
            Imposters.AddRange(Players.Where(pair => pair.Value.Role == Role.Imposter).Select(pair => pair.Key));
            Crewmates.AddRange(Players.Where(pair => pair.Value.Role == Role.Crewmate).Select(pair => pair.Key));
            AlivePlayers.AddRange(Players.Where(pair => pair.Value.Role != Role.None && pair.Value.IsAlive).Select(pair => pair.Key));
            DeadPlayers.AddRange(Players.Where(pair => !(pair.Value.Role != Role.None && pair.Value.IsAlive)).Select(pair => pair.Key));
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
            foreach (var key in Players.Keys.ToList())
            {
                key.Role = RoleType.Spectator;
                Players[key] = new PlayerInfo(this);
            }
        }
        
        public void ClearImposters()
        {
            foreach (Player player in Player.List)
            {
                try
                {
                    if (player.GetInfo().Role == Role.Imposter)
                    {
                        player.Role = RoleType.Spectator;
                    }
                }
                catch(Exception e)
                {
                    continue;
                }
                
            }
        }
        
        public void ClearCrewmates()
        {
            foreach (var player in Crewmates)
            {
                player.Role = RoleType.Spectator;
            }
        }

        public void ClearAllPlayersVotes()
        {
            Log.Debug("Method ClearAllPlayersVotes() invoked.");
            foreach (Player ply in AlivePlayers)
            {
                ply.GetInfo().votes = 0;
                ply.GetInfo().hasVoted = false;
            }
        }

        public void KillMostVotedPlayer()
        {
            int maxVotes = EventHandlers.PlayerManager.AlivePlayers[0].GetInfo().votes;

            int totalVotes = 0;

            int totalSkips = 0;

            int tieVotes = 0;

            Player player = EventHandlers.PlayerManager.AlivePlayers[0];

            
            foreach (Player ply in EventHandlers.PlayerManager.AlivePlayers)
            {
                if (ply.GetInfo().votes > maxVotes)
                {
                    maxVotes = ply.GetInfo().votes;
                    player = ply;
                    totalVotes += ply.GetInfo().votes;
                    if (!ply.GetInfo().hasVoted)
                    {
                        totalSkips++;
                    }
                }
                else if (ply.GetInfo().votes == maxVotes)
                {
                    tieVotes = ply.GetInfo().votes;
                }
            }
            Log.Debug("Max Votes " + maxVotes);
            Log.Debug((totalVotes <= totalSkips || tieVotes == maxVotes).ToString());

            
            if (totalVotes <= totalSkips || tieVotes == maxVotes) return;
            Log.Debug("No return");
            player.SetRole(RoleType.Spectator);
        }
    }
}