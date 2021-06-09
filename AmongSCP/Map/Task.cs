namespace AmongSCP.Map
{
    public class Task
    {
        public string Name;

        public TaskType TaskType;

        public Task(string name, TaskType taskType)
        {
            Name = name;
            TaskType = taskType;
        }
    }
}
