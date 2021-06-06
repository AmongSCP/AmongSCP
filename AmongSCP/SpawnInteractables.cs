using AmongSCP.Map;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            ItemData emergencyInteractableData = new ItemData(ItemType.Flashlight, new Vector3(79.77f, -1005.87f, 40.07f), new Quaternion(0,0,0,0));
            EmergencyMeetingInteractable emergencyInteractable = new EmergencyMeetingInteractable(emergencyInteractableData);
        }

        public void SpawnDeadBodyInteractable(Ragdoll deadBody)
        {
            DeadBodyInteractable deadBodyInteractable = new DeadBodyInteractable(deadBody);
        }
    }
}
