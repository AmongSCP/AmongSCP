using AmongSCP.Map;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmongSCP
{
    public static class TaskManager
    {
        private static List<Task> _possibleTasks = new List<Task>();

        public static List<Task> CurrentTasks = new List<Task>();

        public static Dictionary<Player, List<Task>> PlayerTasks = new Dictionary<Player, List<Task>>();

        public static int TasksCompleted;

        public static void EndGame()
        {
            CurrentTasks.Clear();
            PlayerTasks.Clear();
            _possibleTasks.Clear();
        }

        public static void AddPossibleTasks()
        {
            AddMultipleInstance(20, new Task("Load weapon Manager Tablet into Generator", TaskType.Generator));
            AddMultipleInstance(20, new Task("Interactable in Dogs Room", TaskType.DogRoom));
        }

        public static void SplitTasks()
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

        public static List<Task> GetPlayerTasks(Player ply)
        {
            return PlayerTasks[ply];
        }

        public static bool PlayerCompletedAllTasks(Player ply)
        {
            return PlayerTasks[ply].Count == 0;
        }

        public static bool AllTasksCompleted()
        {
             if(CurrentTasks.Count == 0)
             {
                Exiled.API.Features.Map.Broadcast((ushort)5f,"Crewmates win!");
             }
             return CurrentTasks.Count == 0;
        }

        public static void AddMultipleInstance(int num, Task task)
        {
            for(var i = 0; i < num; i++)
            {
                _possibleTasks.Add(new Task(task.Name, task.TaskType));
            }
        }

        public static Task GetPlayerTask(Player player, TaskType taskType)
        {
            return PlayerTasks.TryGetValue(player, out var arr) ? arr.FirstOrDefault(task => task.TaskType == taskType) : null;
        }

        public static void DeletePlayerTasks(Player ply)
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

        public static void HandleTaskCompletion(Player player, Task task)
        {
            try
            {
                PlayerTasks[player].Remove(task);
                CurrentTasks.Remove(task);
                player.Broadcast((ushort)3f, "Completed the " + task.Name + " task.");
            }
            catch (Exception e)
            {
                throw e;
            }
            if (AllTasksCompleted())
            {
                EventHandlers.PlayerManager.ClearImposters();
            }
        }

        public static bool TryCompletingTask(Player ply, TaskType taskType)
        {
            var task = GetPlayerTask(ply, taskType);

            if (task == null) return false;

            HandleTaskCompletion(ply, task);

            return true;
        }
    }
}
