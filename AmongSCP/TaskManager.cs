using AmongSCP.Map;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmongSCP
{
    public class TaskManager
    {
        private readonly List<Task> _possibleTasks = new List<Task>();

        public readonly List<Task> CurrentTasks = new List<Task>();

        public readonly Dictionary<Player, List<Task>> PlayerTasks = new Dictionary<Player, List<Task>>();

        public int TasksCompleted;

        public TaskManager()
        {
            AddPossibleTasks();
        }

        public void EndGame()
        {
            CurrentTasks.Clear();
            PlayerTasks.Clear();
        }

        public void AddPossibleTasks()
        {
            AddMultipleInstance(40, new Task("Load weapon Manager Tablet into Generator", TaskType.Generator));
        }

        public void SplitTasks()
        {
            _possibleTasks.ShuffleListSecure();

            for(var i = 0; i < EventHandlers.PlayerManager.Crewmates.Count; i++)
            {
                var tasks = _possibleTasks.Skip(i * AmongSCP.Singleton.Config.CrewmateTasks).Take(AmongSCP.Singleton.Config.CrewmateTasks).Select(task => new Task(task.Name, task.TaskType)).ToArray();
                CurrentTasks.AddRange(tasks);
                PlayerTasks[EventHandlers.PlayerManager.Crewmates[i]] = tasks.ToList();
            }
            Log.Debug(CurrentTasks.Count);
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
             if(CurrentTasks.Count == 0)
             {
                Exiled.API.Features.Map.Broadcast((ushort)5f,"Crewmates win!");
             }
             return CurrentTasks.Count == 0;
        }

        public void AddMultipleInstance(int num, Task task)
        {
            for(var i = 0; i < num; i++)
            {
                _possibleTasks.Add(new Task(task.Name, task.TaskType));
            }
        }

        public Task GetPlayerTask(Player player, TaskType taskType)
        {
            return PlayerTasks.TryGetValue(player, out var arr) ? arr.FirstOrDefault(task => task.TaskType == taskType) : null;
        }

        public void DeletePlayerTasks(Player ply)
        {
            foreach(Task task in GetPlayerTasks(ply))
            {
                try
                {
                    CurrentTasks.Remove(task);
                }
                catch
                {

                }
            }
            PlayerTasks.Remove(ply);
        }

        public void HandleTaskCompletion(Player player, Task task)
        {
            try
            {
                PlayerTasks[player].Remove(task);
                CurrentTasks.Remove(task);
            }
            catch 
            {

            }
            if (AllTasksCompleted())
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
