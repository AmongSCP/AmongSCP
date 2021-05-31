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

                player.GetInfo().CalledEmergencyMeeting = true;
                Util.CallEmergencyMeeting(player, player.Nickname + " has called an emergency meeting!");
            });
        }
    }
}