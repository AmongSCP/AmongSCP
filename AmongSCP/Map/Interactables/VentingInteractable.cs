using UnityEngine;

namespace AmongSCP.Map.Interactables
{
    public class VentingInteractable
    {
        private Interactable _interactable;

        private Vector3 _roomPosition;

        private Vector3 _nextRoomPosition;

        public VentingInteractable(Vector3 roomPositon, Vector3 nextRoomPosition)
        {
            _roomPosition = roomPositon;
            _nextRoomPosition = nextRoomPosition;
            ItemData _interactableData = new ItemData(ItemType.Disarmer, _roomPosition, Quaternion.identity, new Vector3(2, 2, 2));
            _interactable = new Interactable(_interactableData, player =>
            {
                if(EventHandlers.PlayerManager.Imposters.Contains(player))
                {
                    player.Position = _nextRoomPosition;
                }
            });
        }
    }
}
