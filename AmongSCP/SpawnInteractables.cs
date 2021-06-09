using AmongSCP.Map;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using AmongSCP.Map.Interactables;
using Exiled.API.Features;

namespace AmongSCP
{
    public class SpawnInteractables
    {
        public SpawnInteractables()
        {
            Log.Debug("no");
            SpawnEmergencyMeetingInteractable();
        }

        public void SpawnEmergencyMeetingInteractable()
        {
            var emergencyInteractableData = new ItemData(ItemType.Flashlight, new Vector3(79.77f, -1005.87f, 40.07f), new Quaternion(0, 0, 0, 0));
            var emergencyInteractable = new EmergencyMeetingInteractable(emergencyInteractableData);
        }

        public void SpawnDeadBodyInteractable(Vector3 pos, string playerName)
        {
            var deadBodyInteractable = new DeadBodyInteractable(pos, playerName);
        }

        public void SpawnVentInteractable(Dictionary<Vector3, Vector3> cordinates)
        {
            for (int i = 0; i < cordinates.Count; i++)
            {
                var element = cordinates.ElementAt(i);
                var vent = new VentingInteractable(element.Key, element.Value);
            }

        }
    }
}