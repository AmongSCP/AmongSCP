using System;
using AmongSCP.Map;
using Exiled.API.Enums;
using Exiled.Events.EventArgs;
using MEC;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Interactables.Interobjects.DoorUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AmongSCP
{
    public class TaskManager
    {
        public int _totalTasks;

        public int _tasksCompleted;

        public bool TasksComplete()
        {
            return _totalTasks == _tasksCompleted;
        }

        public void OnGeneratorEngaged()
        {
            _tasksCompleted++;
        }

        public void GenerateTasks()
        {

        }


    }
}
