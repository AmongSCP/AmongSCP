using UnityEngine;
using System;

namespace AmongSCP.Map.Interactables
{
    public class VentingInteractable
    {
        private Interactable _interactable;

        private MapPosition _roomPosition;

        private MapPosition _nextRoomPosition;

        public VentingInteractable(MapPosition roomPositon, MapPosition nextRoomPosition)
        {
            _roomPosition = roomPositon;
            _nextRoomPosition = nextRoomPosition;
            var interactableData = new ItemData(ItemType.Radio, _roomPosition, Quaternion.identity, new Vector3(2, 2, 2));
            _interactable = new Interactable(interactableData, player =>
            {
                var playerInfo = player.GetInfo();

                if (playerInfo.Role != PlayerManager.Role.Imposter) return;

                player.Position = _nextRoomPosition.GetRealPosition();
            });
        }
    }
}
