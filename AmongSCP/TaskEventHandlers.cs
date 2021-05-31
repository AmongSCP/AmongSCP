using System;
using AmongSCP.Map;
using Exiled.API.Enums;
using Exiled.Events.EventArgs;
using MEC;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Interactables.Interobjects.DoorUtils;
using System.Linq;
using UnityEngine;

namespace AmongSCP
{
    public class TaskEventHandlers
    {
        private static TaskManager _taskManager;

        public TaskEventHandlers(TaskManager taskManager)
        {
            _taskManager = taskManager;
        }

        public static void OnInsertingGeneratorTabletEvent(InsertingGeneratorTabletEventArgs ev)
        {
            
        }

        public static void HandleGeneratorEngaged(GeneratorActivatedEventArgs ev)
        {

        }

        
    }
}
