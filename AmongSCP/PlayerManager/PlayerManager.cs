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
            foreach (var player in Imposters.ToList())
            {
                player.Role = RoleType.Spectator;
            }
        }
        
        public void ClearCrewmates()
        {
            foreach (var player in Crewmates.ToList())
            {
                player.Role = RoleType.Spectator;
            }
        }

        public void ClearAllPlayersVotes()
        {
            foreach (var ply in AlivePlayers)
            {
                var info = ply.GetInfo();
                
                info.votes = 0;
                info.hasVoted = false;
            }
        }

        public void KillMostVotedPlayer()
        {
            var votes = EventHandlers.PlayerManager.AlivePlayers.OrderByDescending(ply => ply.GetInfo().votes).ToList();
            var skips = EventHandlers.PlayerManager.AlivePlayers.Count(ply => ply.GetInfo().skipped || !ply.GetInfo().hasVoted);

            var first = votes.Count > 0 ? votes[0].GetInfo().votes : 0;
            var second = votes.Count > 1 ? votes[1].GetInfo().votes : 0;
            
            //If there are no votes or first is tied or there are >= skips to first
            if (votes.Count < 1 || first <= second || skips >= first)
            {
                Exiled.API.Features.Map.Broadcast((ushort)3f, "No one was Ejected.");
                return;
            }

            Exiled.API.Features.Map.Broadcast((ushort)3f, votes[0].Nickname + "was a " + votes[0].GetInfo().Role);
            votes[0].Kill();
        }
    }
}