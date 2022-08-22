using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Exiled.API.Features;

namespace AmongSCP.Map.TaskInteractables
{
    public class DogRoomTaskInteractable
    {
        private Interactable __interactable;

        public DogRoomTaskInteractable()
        {
            ItemData DogRoomTaskItemData = new ItemData(ItemType.MicroHID, new Vector3(91.35767f, -1012.465f, 57.9607f), Quaternion.identity, new Vector3(2, 2, 2));

            __interactable = new Interactable(DogRoomTaskItemData, ply =>
            {
                if (ply.GetInfo().Role == PlayerManager.Role.Imposter) return;
                ply.GetInfo().dogTaskInteractions++;
                if (ply.GetInfo().dogTaskInteractions == 8)
                {
                    TaskManager.TryCompletingTask(ply, TaskType.DogRoom);
                    return;
                }

                if (__interactable.Pickup.Position.Equals(new Vector3(91.35767f, -1012.465f, 52.03667f)))
                {
                    __interactable.Pickup.Position = new Vector3(91.35767f, -1012.465f, 57.9607f); 
                    return;
                }
                __interactable.Pickup.Position = new Vector3(91.35767f, -1012.465f, 52.03667f);
            });
        }
    }
}
