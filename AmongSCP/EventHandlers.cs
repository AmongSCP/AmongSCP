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

        private readonly PlayerManager _playerManager = new PlayerManager();

        public EventHandlers(AmongSCP plugin)
        {
            _plugin = plugin;
        }
        
        //TODO - Item pickup events
        public void OnPickupItem(PickingUpItemEventArgs ev)
        {

        }

        //TODO - Work on voting
        private void StartVoting()
        {
            
        }

        //TODO - Figure out how to report bodies
        public void ReportBody()
        {
            
        }

        public void OnDied(DiedEventArgs ev)
        {
           Log.Debug($"Someone died at: {ev.Target.Position.x}, {ev.Target.Position.y}, {ev.Target.Position.z}");
        }
        
        public void OnGameStart()
        {
            Timing.CallDelayed(.2f, () =>
            {
                _playerManager.UpdateQueueNoWait();

                var players = _playerManager.PickPlayers(_plugin.Config.MaxPlayers);

                _playerManager.ClearQueued();

                players.ShuffleList();
                
                Log.Info(players);
                Log.Info(players.Length);

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

            _playerManager.UpdateQueue();
        }

        public void OnLeave(LeftEventArgs ev)
        {
            if (!RoundSummary.RoundInProgress()) return;

            _playerManager.UpdateQueue();
        }

        private void ChangeOutfit(Player ply, RoleType type)
        {
            Log.Debug(ply.Nickname + " is an SCP!");
            foreach (var target in Player.List.Where(x => x != ply))
            {
                if(_playerManager.Imposters.Contains(ply))
                {
                    MirrorExtensions.SendFakeSyncVar(target, ply.ReferenceHub.networkIdentity, typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkCurClass), (sbyte)type);
                }
            }
        }

        private void MakePlayerCrewmate(Player ply)
        {
            ply.SetRole(_plugin.Config.CrewmateRole);
        }

        private void MakePlayerImposter(Player ply)
        {
            ply.SetRole(_plugin.Config.ImposterRole);
            Timing.CallDelayed(.1f, () => ChangeOutfit(ply, _plugin.Config.CrewmateRole));
        }
    }
}