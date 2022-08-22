using AmongSCP.Map;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using AmongSCP.Map.Interactables;
using Exiled.API.Features;
using AmongSCP.Map.TaskInteractables;

namespace AmongSCP
{
    public class SpawnInteractables
    {
        public SpawnInteractables()
        {
            SpawnEmergencyMeetingInteractable();
            SpawnVentInteractables(PointManager.Vents);
            SpawnLightsInteractable(PointManager.LightsSpawn);
            SpawnDogRoomTask();
        }

        public void SpawnEmergencyMeetingInteractable()
        {
            var emergencyInteractableData = new ItemData(ItemType.Flashlight, new Vector3(72.2f, -1005.87f, 159.2f), new Quaternion(0, 0, 0, 0));
            var emergencyInteractable = new EmergencyMeetingInteractable(emergencyInteractableData);
        }

        public void SpawnDeadBodyInteractable(Vector3 pos, string playerName)
        {
            var deadBodyInteractable = new DeadBodyInteractable(pos, playerName);
        }

        public void SpawnVentInteractables(Dictionary<Vector3, Vector3> cordinates)
        {
            for (int i = 0; i < cordinates.Count; i++)
            {
                var element = cordinates.ElementAt(i);
                var vent = new VentingInteractable(element.Key, element.Value);
            }
        }

        public void SpawnLightsInteractable(Vector3 pos)
        {
            var lightsInteractable = new TurnOnLightsInteractable(pos, ItemType.GrenadeFlash);
        }

        public void SpawnDogRoomTask()
        {
            var task = new DogRoomTaskInteractable();
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