using AmongSCP.Map;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using AmongSCP.Map.Interactables;
using Exiled.API.Features;
using Exiled.API.Enums;

namespace AmongSCP
{
    public class SpawnInteractables
    {
        public SpawnInteractables()
        {
            SpawnEmergencyMeetingInteractable();
            SpawnVentInteractables(PointManager.Vents);
            SpawnLightsInteractable(PointManager.LightsSpawn);
        }

        public void SpawnEmergencyMeetingInteractable()
        {
            var emergencyInteractableData = new ItemData(ItemType.Flashlight, new MapPosition(-6.8f, -5.9f, -9.6f, RoomType.Hcz079), new Quaternion(0, 0, 0, 0));
            var emergencyInteractable = new EmergencyMeetingInteractable(emergencyInteractableData);
        }

        public void SpawnDeadBodyInteractable(MapPosition pos, string playerName)
        {
            var deadBodyInteractable = new DeadBodyInteractable(pos, playerName);
        }

        public void SpawnVentInteractables(Dictionary<MapPosition, MapPosition> cordinates)
        {
            for (int i = 0; i < cordinates.Count; i++)
            {
                var element = cordinates.ElementAt(i);
                var vent = new VentingInteractable(element.Key, element.Value);
            }
        }

        public void SpawnLightsInteractable(MapPosition pos)
        {
            var lightsInteractable = new TurnOnLightsInteractable(pos, ItemType.GrenadeFlash);
        }

        public void SpawnHats(Player[] players)
        {
            var possibleItems = new List<ItemType>()
            {
                ItemType.KeycardChaosInsurgency,
                ItemType.KeycardContainmentEngineer,
                ItemType.KeycardFacilityManager,
                ItemType.KeycardGuard,
                ItemType.KeycardJanitor,
                ItemType.KeycardO5,
                ItemType.KeycardNTFLieutenant,
                ItemType.KeycardScientist,
                ItemType.KeycardNTFOfficer,
                ItemType.KeycardZoneManager
            };

            possibleItems.ShuffleListSecure();

            var count = 0;
            foreach (var ply in players)
            {
                var hatInteractable = new HatInteractable(ply, possibleItems[count]);
                count++;
            }
        }
    }
}