using Exiled.Events.EventArgs;
using MEC;

namespace AmongSCP
{
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using System.Collections.Generic;
    using System.Linq;

    public class EventHandlers
    {
        private readonly AmongSCP _plugin;

        private List<Player> _imposters = new List<Player>();  
        
        private readonly Queue _queue = new Queue();

        public EventHandlers(AmongSCP plugin)
        {
            _plugin = plugin;
        }

        public void OnGameStart()
        {
            Timing.CallDelayed(.2f, () =>
            {
                _queue.UpdateQueueNoWait();

                var players = _queue.PickPlayers();
                players.ShuffleList();
                
                for (var i = 0; i < players.Length; i++)
                {
                    if (i % 5 < _plugin.Config.ImposterRatio)
                    {
                        MakePlayerImposter(players[i]);
                    }
                    else
                    {
                        MakePlayerCrewmate(players[i]);
                    }
                }
            });
        }

        public void OnJoin(VerifiedEventArgs ev)
        {
            if (!RoundSummary.RoundInProgress()) return;

            _queue.UpdateQueue();
        }

        public void OnLeave(LeftEventArgs ev)
        {
            if (!RoundSummary.RoundInProgress()) return;

            _queue.UpdateQueue();
        }

        private void ChangeOutfit(Player ply, RoleType type)
        {
            foreach (var target in Player.List.Where(x => x != ply))
            {
                if(_imposters.Contains(ply))
                {
                    MirrorExtensions.SendFakeSyncVar(target, ply.ReferenceHub.networkIdentity, typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkCurClass), (sbyte)type);
                }
            }
        }

        private void MakePlayerCrewmate(Player ply)
        {
            
        }

        private void MakePlayerImposter(Player ply)
        {
            
        }
    }
}