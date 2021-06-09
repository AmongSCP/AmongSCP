using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Features;
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
                HandleInteraction(player);
            });
        }

        public void HandleInteraction(Player player)
        {
            player.Position = _nextRoomPosition;
        }



    }
}
