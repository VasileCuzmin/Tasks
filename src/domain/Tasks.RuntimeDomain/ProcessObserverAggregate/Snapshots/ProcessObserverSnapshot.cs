using System.Collections.Generic;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;

namespace Tasks.Runtime.Domain.ProcessObserverAggregate.Snapshots
{
    public class ProcessObserverSnapshot
    {
        public ProcessObserverId ProcessObserverId { get; }
        public IEnumerable<TaskDefinition> TaskDefinitions { get; }
        public IEnumerable<JsonEvent> ReceivedEvents { get; }
        public IEnumerable<Task> Tasks { get; }

        public ProcessObserverSnapshot(ProcessObserverId processObserverId, IEnumerable<TaskDefinition> taskDefinitions,
            IEnumerable<JsonEvent> receivedEvents, IEnumerable<Task> tasks)
        {
            ProcessObserverId = processObserverId;
            TaskDefinitions = taskDefinitions;
            ReceivedEvents = receivedEvents;
            Tasks = tasks;
        }

        public class Task
        {
            public TaskId TaskId { get; }
            public TaskDefinition TaskDefinition { get; }
            public TaskState State { get; }

            public Task(TaskId taskId, TaskDefinition taskDefinition, TaskState state)
            {
                TaskId = taskId;
                TaskDefinition = taskDefinition;
                State = state;
            }
        }
    }
}
