﻿using Exiled.Events.EventArgs;
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

        public void OnGameStart()
        {
            Timing.CallDelayed(.2f, () =>
            {
                _playerManager.UpdateQueueNoWait();

                var players = _playerManager.PickPlayers();
                players.ShuffleList();
                
                _playerManager.UpdateQueueNoWait();
                
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
            
        }

        private void MakePlayerImposter(Player ply)
        {
            ply.SetRole(RoleType.Scp049);
            Timing.CallDelayed(0.1f, () => ChangeOutfit(ply, RoleType.NtfLieutenant));
        }
    }
}