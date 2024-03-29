﻿using Exiled.API.Features;

namespace AmongSCP.Map.Interactables
{
    public class HatInteractable
    {
        public Player Player;

        public ItemType item;

        public Interactable interactable;

        public HatInteractable(Player ply, ItemType i)
        {
            Player = ply;

            item = i;

            var itemdata = new ItemData(item, new MapPosition(0, 0, 0), UnityEngine.Quaternion.identity, new UnityEngine.Vector3(4,8,4));
            interactable = new Interactable(itemdata, player =>
            {
                Log.Debug(Util.meetingStarted.ToString());
                Log.Debug((!player.GetInfo().hasVoted).ToString());

                if (!Util.meetingStarted || player.GetInfo().hasVoted) return;
                Player.GetInfo().votes++;
                player.GetInfo().hasVoted = true;
                player.Broadcast((ushort)5f, "You have voted for " + Player.Nickname);
                Util.VoteAmount++;
            });
            SCPStats.API.API.SpawnHat(Player, interactable.Pickup, new UnityEngine.Vector3(0f, .1f, 0f), UnityEngine.Quaternion.identity);
        }
    }
}
