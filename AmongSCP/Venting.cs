using AmongSCP.Map;
using Exiled.API.Enums;
using Exiled.Events.EventArgs;
using MEC;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Interactables.Interobjects.DoorUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AmongSCP
{
    class Venting
    {
        private static void PlayerVent(Player ply, string door)
        {
            DoorVariant CurDooor = Exiled.API.Features.Map.GetDoorByName(door);
            Vector3 doorPos = CurDooor.transform.position;
            ply.Position = doorPos;
        }
    }
}
