using AmongSCP.Map;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmongSCP
{
    public class TaskManager
    {
        private List<Task> PossibleTasks = new List<Task>();

        public List<Task> CurrentTasks = new List<Task>();

        public Dictionary<Player, List<Task>> PlayerTasks = new Dictionary<Player, List<Task>>();

        public int TasksCompleted;

        public TaskManager()
        {
            AddPossibleTasks();
        }

        public void AddPossibleTasks()
        {
            AddMultipleInstance(5, new Task("Load weapon Manager Tablet into Generator", TaskType.Generator));
        }

        public void SplitTasks()
        {
            PossibleTasks.ShuffleListSecure();

            for(var i = 0; i < EventHandlers.PlayerManager.Crewmates.Count; i++)
            {
                var tasks = PossibleTasks.Skip(i * AmongSCP.Singleton.Config.CrewmateTasks).Take(AmongSCP.Singleton.Config.CrewmateTasks).ToArray();
                CurrentTasks.AddRange(tasks);
                PlayerTasks[EventHandlers.PlayerManager.Crewmates[i]] = tasks.ToList();
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
            for(var i = 0; i < num; i++)
            {
                PossibleTasks.Add(new Task(task.Name, task.TaskType));
            }
        }

        public Task GetPlayerTask(Player player, TaskType taskType)
        {
            return PlayerTasks.TryGetValue(player, out var arr) ? arr.FirstOrDefault(task => task.TaskType == taskType) : null;
        }

        public void HandleTaskCompletion(Player player, Task task)
        {
            PlayerTasks[player].Remove(task);
            CurrentTasks.Remove(task);

            if(AllTasksCompleted())
            {
                EventHandlers.PlayerManager.ClearImposters();
            }
        }

        public bool TryCompletingTask(Player ply, TaskType taskType)
        {
            var task = GetPlayerTask(ply, taskType);
            if (task == null) return false;

            HandleTaskCompletion(ply, task);

            return true;
        }
    }
}
