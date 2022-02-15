using NBB.Domain;
using Newtonsoft.Json;

namespace Tasks.Runtime.Domain.ProcessObserverAggregate
{
    public class TaskState : Enumeration
    {
        public static readonly TaskState Initiated = new TaskState(0, "Initiated");
        public static readonly TaskState Finished = new TaskState(4, "Finished");
        public static readonly TaskState Canceled = new TaskState(5, "Canceled");
        
        [JsonConstructor]
        public TaskState(int id, string name)
            : base(id, name)
        {
        }
    }
}