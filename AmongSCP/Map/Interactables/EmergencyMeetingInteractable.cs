using MEC;
using System.Collections.Generic;

namespace AmongSCP.Map.Interactables
{
    public class EmergencyMeetingInteractable
    {
        private Interactable _interactable;

        public EmergencyMeetingInteractable(ItemData data)
        {
            _interactable = new Interactable(data, player =>
            {
                if (player.GetInfo().CalledEmergencyMeeting)
                {
                    player.ShowHint("You have already called an emergency meeting!");
                    return;
                }
                else if(player.GetInfo().EmergencyMeetings == 0)
                {
                    player.ShowHint("You have used all of your emergency Meetings!");
                    return;
                }

                player.GetInfo().CalledEmergencyMeeting = true;
                Timing.RunCoroutine(Util.CallEmergencyMeeting(player, player.Nickname + " has called an emergency meeting!", false));
                player.GetInfo().EmergencyMeetings -= 1;
            });
        }
    }
}