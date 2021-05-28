using Exiled.Events.EventArgs;
using MEC;

namespace AmongSCP
{
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    
    public class EventHandlers
    {
        private readonly AmongSCP _plugin;
        
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

        private static void MakePlayerImposter(Player ply)
        {
            ply.SetRole(RoleType.Scp049);
            ply.ChangeAppearance(RoleType.NtfLieutenant);
        }

        private static void MakePlayerCrewmate(Player ply)
        {
            
        }
    }
}