using MEC;
using UnityEngine;

namespace AmongSCP.Map.Interactables
{
    public class DeadBodyInteractable
    {
        private Interactable _interactable;

        public DeadBodyInteractable(Vector3 pos, string name)
        {
            var deadBodyPosition = pos;
            deadBodyPosition.y += 1;

            var deadBodyItemData = new ItemData(ItemType.SCP018, deadBodyPosition, Quaternion.identity, new Vector3(2, 2, 2));

            _interactable = new Interactable(deadBodyItemData, player =>
            {
                Timing.RunCoroutine(Util.CallEmergencyMeeting(player, player.Nickname + " has reported the body of" + name + "!", true));
            }, true);
        }
    }
}
