using AmongSCP.Map;
using Exiled.API.Features;
using System.Collections.Generic;

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
