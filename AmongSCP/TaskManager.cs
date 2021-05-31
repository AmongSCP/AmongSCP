using AmongSCP.Map;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmongSCP
{
    public class TaskManager
    {   
        private PlayerManager _playerManager;

        private List<Task> PossibleTasks = new List<Task>();

        public List<Task> CurrentTasks = new List<Task>();

        public Dictionary<Player, List<Task>> PlayerTasks = new Dictionary<Player, List<Task>>();

        public TaskManager(PlayerManager playerManager)
        {
            _playerManager = playerManager;
            AddPossibleTasks();
        }

        public void AddPossibleTasks()
        {
            AddMultipleInstance(5, new Task("Load weapon Manager Tablet into Generator", TaskType.GENERATOR));
        }

        public void SplitTasks()
        {
            PossibleTasks.ShuffleListSecure();
            for(int i = 0; i < _playerManager.Crewmates.Count; i++)
            {
                var tasks = PossibleTasks.Skip(i * AmongSCP.Singleton.Config.CrewmateTasks).Take(AmongSCP.Singleton.Config.CrewmateTasks);
                CurrentTasks.AddRange(tasks);
                PlayerTasks[_playerManager.Crewmates[i]] = tasks.ToList();
            }
        }

        public void ShowPlayerTasks(Player ply)
        {

        }

        public List<Task> GetPlayerTasks(Player ply)
        {
            return PlayerTasks[ply];
        }


        public bool PlayerCompletedAllTasks(Player ply)
        {
            return PlayerTasks[ply].Count == 0;
        }

        public bool AllTasksCompleted()
        {
            return CurrentTasks.Count == 0;
        }

        public void AddMultipleInstance(int num, Task task)
        {
            for(int i = 0; i < num; i++)
            {
                PossibleTasks.Add(new Task(task.Name, task.TaskType));
            }
        }

        public bool PlayerCanCompleteTask(Player player, Task task)
        {
            if(!PlayerTasks[player].Contains(task)) return false;
            return true;

        }

        public void HandleTaskCompletion(Player player, Task task)
        {
            try
            {
                PlayerTasks[player].Remove(task);
                CurrentTasks.Remove(task);
            }
            catch (Exception e)
            {
                return;
            }
            
        }
    }
}
