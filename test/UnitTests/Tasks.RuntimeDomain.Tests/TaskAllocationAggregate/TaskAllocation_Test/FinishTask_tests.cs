using System.Linq;
using FluentAssertions;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessObserverAggregate;
using Tasks.Runtime.Domain.TaskAllocationAggregate;
using Tasks.Runtime.Domain.TaskAllocationAggregate.DomainEvents;
using Xunit;

namespace Tasks.RuntimeDomain.Tests.TaskAllocationAggregate.TaskAllocation_Test
{
    public class FinishTask_tests
    {
        [Fact]
        public void FinishTask()
        {
            // Arrange
            var keyValue = new ImmutableKeyValue(string.Empty, string.Empty);
            var processId = new ProcessId(keyValue);
            var taskId = new TaskId();
            var sut = Setup.GenerateTaskAllocationAggregate(new TaskDefinition("TaskDefinition004", Setup.bad, Setup.bad, Setup.good, null, null, null, null, null, new AutomaticStart(true)), taskId, processId);

            //Act
            sut.Finish(processId);

            // Assert
            Assert.Equal(sut.State, TaskAllocationStatus.Finished);
            var emittedEvents = sut.GetUncommittedChanges().ToList();
            emittedEvents.Where(e => e is TaskFinished).Should().HaveCount(1);
        }
    }
}