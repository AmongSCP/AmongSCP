using UnityEngine;
using MEC;

namespace AmongSCP.Map.Interactables
{
    public class VentingInteractable
    {
        private Interactable _interactable;

        private Vector3 _roomPosition;

        private Vector3 _nextRoomPosition;

        private static bool CanVent = true;

        public VentingInteractable(Vector3 roomPositon, Vector3 nextRoomPosition)
        {
            _roomPosition = roomPositon;
            _nextRoomPosition = nextRoomPosition;
            ItemData _interactableData = new ItemData(ItemType.Disarmer, _roomPosition, Quaternion.identity, new Vector3(2, 2, 2));
            _interactable = new Interactable(_interactableData, player =>
            {
                if(EventHandlers.PlayerManager.Imposters.Contains(player))
                {
                    if(CanVent)
                    {
                        player.Position = _nextRoomPosition;
                        CanVent = false;
                        Timing.CallDelayed(AmongSCP.Singleton.Config.VentKillCooldown, () => CanVent = true);
                    }
                }
            });
        }
    }
}
