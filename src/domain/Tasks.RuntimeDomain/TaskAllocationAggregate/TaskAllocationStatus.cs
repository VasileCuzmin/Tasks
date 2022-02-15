using NBB.Domain;
using Newtonsoft.Json;

namespace Tasks.Runtime.Domain.TaskAllocationAggregate
{
    public class TaskAllocationStatus : Enumeration
    {
        public static readonly TaskAllocationStatus Created = new TaskAllocationStatus(1, "Created");
        public static readonly TaskAllocationStatus Allocated = new TaskAllocationStatus(2, "Allocated");
        public static readonly TaskAllocationStatus InProgress = new TaskAllocationStatus(3, "InProgress");
        public static readonly TaskAllocationStatus InStandby = new TaskAllocationStatus(4, "InStandby");
        public static readonly TaskAllocationStatus Finished = new TaskAllocationStatus(5, "Finished");
        public static readonly TaskAllocationStatus Cancelled = new TaskAllocationStatus(6, "Cancelled");

        [JsonConstructor]
        public TaskAllocationStatus(int id, string name)
            : base(id, name)
        {
        }
    }
}
