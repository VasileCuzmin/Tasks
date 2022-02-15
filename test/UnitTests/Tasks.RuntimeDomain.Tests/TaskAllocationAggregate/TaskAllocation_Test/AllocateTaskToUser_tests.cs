using System;
using FluentAssertions;
using NBB.Domain;
using Tasks.Runtime.Domain.Constants;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessObserverAggregate;
using Tasks.Runtime.Domain.TaskAllocationAggregate;
using Xunit;

namespace Tasks.RuntimeDomain.Tests.TaskAllocationAggregate.TaskAllocation_Test
{
    public class AllocateTaskToUser_tests
    {
        [Fact]
        public void AllocateTaskToUser_TaskIsCancelled_ThrowsDomainException()
        {
            // Arrange
            var keyValue = new ImmutableKeyValue(string.Empty, string.Empty);
            var processId = new ProcessId(keyValue);
            var taskId = new TaskId();
            var sut = Setup.GenerateTaskAllocationAggregate(new TaskDefinition("TaskDefinition004", Setup.bad, Setup.bad, Setup.good, null, null, null, null, null, new AutomaticStart(true)), taskId, processId);
            sut.Cancel(processId);
            //Act
            var allocateTask = new Action(() => sut.AllocateUser(processId.ToString(), 1));

            // Assert
            Assert.Throws<DomainException>(allocateTask).Message.Should().Be(ErrorCodes.TaskFinnishedOrCancelled.ToString());
        }

        [Fact]
        public void AllocateTaskToUser_TaskIsCreated_StateOfTaskBecomeAllocatedAndReturnsAllocatedUser()
        {
            // Arrange
            var keyValue = new ImmutableKeyValue(string.Empty, string.Empty);
            var processId = new ProcessId(keyValue);
            var taskId = new TaskId();
            var sut = Setup.GenerateTaskAllocationAggregate(new TaskDefinition("TaskDefinition004", Setup.bad, Setup.bad, Setup.good, null, null, null, null, null, new AutomaticStart(false)), taskId, processId);

            //Act
            sut.AllocateUser(processId.ToString(), 2);

            // Assert
            Assert.Equal(sut.State, TaskAllocationStatus.Allocated);
            Assert.Equal(2, sut.UserId);
        }

        [Fact]
        public void AllocateTaskToUser_TaskIsInStandBy_StateOfTaskBecomeAllocatedAndReturnsAllocatedUser()
        {
            // Arrange
            var keyValue = new ImmutableKeyValue(string.Empty, string.Empty);
            var processId = new ProcessId(keyValue);
            var taskId = new TaskId();
            var sut = Setup.GenerateTaskAllocationAggregate(new TaskDefinition("TaskDefinition004", Setup.bad, Setup.bad, Setup.good, null, null, null, null, null, new AutomaticStart(false)), taskId, processId);
            sut.AllocateUser(processId.ToString(), 1);
            sut.Pause(processId.ToString());

            //Act
            sut.AllocateUser(processId.ToString(), 2);

            // Assert
            Assert.Equal(sut.State, TaskAllocationStatus.Allocated);
            Assert.Equal(2, sut.UserId);
        }
    }
}