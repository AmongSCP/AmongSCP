using System.Collections;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Enums;
using Exiled.Events.EventArgs;
using System.Linq;
using UnityEngine;
using Random = System.Random;
using CustomPlayerEffects;
using Mirror;
using Exiled.API.Extensions;

namespace AmongSCP
{
    public class EventHandlers
    {
        private readonly AmongSCP _plugin;

        public EventHandlers(AmongSCP plugin)
        {
            _plugin = plugin;
        }
        
        public void OnGameStart()
        {
            foreach(var ply in Player.List)
            {
                MakePlayerImposter(ply);
                break;
            }
        }

        private void MakePlayerImposter(Player ply)
        {
            ply.SetRole(RoleType.Scp049);
            ply.ChangeAppearance(RoleType.NtfLieutenant);
        }
    }
}