namespace AmongSCP.Map.Interactables
{
    public class EmergencyMeetingInteractable
    {
        private Interactable _interactable;
        
        public EmergencyMeetingInteractable(ItemData data)
        {
            _interactable = new Interactable(data, player =>
            {
                if (EventHandlers.PlayerManager.CalledEmergencyMeeting.Contains(player))
                {
                    player.ShowHint("You have already called an emergency meeting!");
                    return;
                }
                
                EventHandlers.PlayerManager.CalledEmergencyMeeting.Add(player);
                Util.CallEmergencyMeeting(player, player.Nickname + " has called an emergency meeting!");
            });
        }
    }
}