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
        private PlayerManager _playerManager;

        public List<Task> CurrentTasks = new List<Task>();

        public Dictionary<Player, List<Task>> PlayerTasks = new Dictionary<Player, List<Task>>();

        public TaskManager(PlayerManager playerManager)
        {
            _playerManager = playerManager;
            AddPossibleTasks();
        }

        public void AddPossibleTasks()
        {

        }

        public void GenerateRandomTasks()
        {
            
        }

        public void SplitTasks()
        {

        }

        public void ShowPlayerTasks(Player ply)
        {

        }

        public List<bool> GetPlayerTasks(Player ply)
        {
            return new List<bool>();
        }


        public bool PlayerCompletedAllTasks(Player ply)
        {
            return false;
        }

        public bool AllTasksCompleted()
        {
            return false;
        }
    }
}
