using System.Linq;
using FluentAssertions;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessObserverAggregate;
using Tasks.Runtime.Domain.TaskAllocationAggregate;
using Tasks.Runtime.Domain.TaskAllocationAggregate.DomainEvents;
using Xunit;

namespace Tasks.RuntimeDomain.Tests.TaskAllocationAggregate.TaskAllocation_Test
{
    public class CancelTask_tests
    {
        [Fact]
        public void CancelTask()
        {
            // Arrange
            var keyValue = new ImmutableKeyValue(string.Empty, string.Empty);
            var processId = new ProcessId(keyValue);
            var taskId = new TaskId();
            var sut = Setup.GenerateTaskAllocationAggregate(new TaskDefinition("TaskDefinition004", Setup.bad, Setup.bad, Setup.good, null, null, null, null, null, new AutomaticStart(true)), taskId, processId);

            //Act
            sut.Cancel(processId);

            // Assert
            Assert.Equal(sut.State, TaskAllocationStatus.Cancelled);
            var emittedEvents = sut.GetUncommittedChanges().ToList();
            emittedEvents.Where(e => e is TaskCancelled).Should().HaveCount(1);
        }
    }
}