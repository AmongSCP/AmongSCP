using UnityEngine;
using System;

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
            var interactableData = new ItemData(ItemType.Radio, _roomPosition, Quaternion.identity, new Vector3(2, 2, 2));
            _interactable = new Interactable(interactableData, player =>
            {
                var playerInfo = player.GetInfo();

                if (playerInfo.Role != PlayerManager.Role.Imposter) return;

                var lastVent = playerInfo.LastVent;
                var seconds = lastVent == DateTime.MinValue ? (AmongSCP.Singleton.Config.VentTime + 1) : (int) DateTime.Now.Subtract(lastVent).TotalSeconds;

                if(seconds <= AmongSCP.Singleton.Config.VentTime)
                {
                    player.ShowHint("You are still on cooldown. " + (AmongSCP.Singleton.Config.VentTime - seconds) + " seconds left.");
                    return;
                }

                playerInfo.LastVent = DateTime.Now;
                player.Position = _nextRoomPosition;
            });
        }
    }
}
