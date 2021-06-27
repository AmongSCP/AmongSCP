using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;

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

            var itemdata = new ItemData(item, new UnityEngine.Vector3(72.4817f,-1005.853f,34.69f), UnityEngine.Quaternion.identity, new UnityEngine.Vector3(4,8,4));
            interactable = new Interactable(itemdata, player =>
            {
                Log.Debug(Util.meetingStarted.ToString());
                Log.Debug((!player.GetInfo().hasVoted).ToString());
                if(Util.meetingStarted && !player.GetInfo().hasVoted)
                {
                    Log.Debug("voted");
                    Player.GetInfo().votes++;
                    player.GetInfo().hasVoted = true;
                    Util.VoteAmount++;
                }
            });
            SCPStats.API.API.SpawnHat(Player, interactable.Pickup, new UnityEngine.Vector3(0f, .1f, 0f), UnityEngine.Quaternion.identity);
        }


    }
}
