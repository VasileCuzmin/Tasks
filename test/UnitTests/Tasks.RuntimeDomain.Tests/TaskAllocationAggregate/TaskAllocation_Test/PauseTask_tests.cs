using System;
using System.Linq;
using FluentAssertions;
using NBB.Domain;
using Tasks.Runtime.Domain.Constants;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessObserverAggregate;
using Tasks.Runtime.Domain.TaskAllocationAggregate;
using Tasks.Runtime.Domain.TaskAllocationAggregate.DomainEvents;
using Xunit;

namespace Tasks.RuntimeDomain.Tests.TaskAllocationAggregate.TaskAllocation_Test
{
    public class PauseTask_tests
    {
        [Fact]
        public void PauseTask_TaskIsAlreadyInStandBy_ThrowsDomainException()
        {
            // Arrange
            var keyValue = new ImmutableKeyValue(string.Empty, string.Empty);
            var processId = new ProcessId(keyValue);
            var taskId = new TaskId();
            var sut = Setup.GenerateTaskAllocationAggregate(new TaskDefinition("TaskDefinition004", Setup.bad, Setup.bad, Setup.good, null, null, null, null, null, new AutomaticStart(true)), taskId, processId);
            sut.AllocateUser(processId.ToString(), 1);
            sut.Pause(processId.ToString());
            //Act
            var pauseTask = new Action(() => sut.Pause(processId.ToString()));

            // Assert
            Assert.Throws<DomainException>(pauseTask).Message.Should().Be(ErrorCodes.TaskAlreadyInStandby.ToString());
        }

        [Fact]
        public void PauseTask_TaskIsFinished_ThrowsDomainException()
        {
            // Arrange
            var keyValue = new ImmutableKeyValue(string.Empty, string.Empty);
            var processId = new ProcessId(keyValue);
            var taskId = new TaskId();
            var sut = Setup.GenerateTaskAllocationAggregate(new TaskDefinition("TaskDefinition004", Setup.bad, Setup.bad, Setup.good, null, null, null, null, null, new AutomaticStart(true)), taskId, processId);
            sut.Finish(processId);

            //Act
            var pauseTask = new Action(() => sut.Pause(processId.ToString()));

            // Assert
            Assert.Throws<DomainException>(pauseTask).Message.Should().Be(ErrorCodes.TaskFinnishedOrCancelled.ToString());
        }

        [Fact]
        public void PauseTask_TaskIsCancelled_ThrowsDomainException()
        {
            // Arrange
            var keyValue = new ImmutableKeyValue(string.Empty, string.Empty);
            var processId = new ProcessId(keyValue);
            var taskId = new TaskId();
            var sut = Setup.GenerateTaskAllocationAggregate(new TaskDefinition("TaskDefinition004", Setup.bad, Setup.bad, Setup.good, null, null, null, null, null, new AutomaticStart(true)), taskId, processId);
            sut.Cancel(processId);

            //Act
            var pauseTask = new Action(() => sut.Pause(processId.ToString()));

            // Assert
            Assert.Throws<DomainException>(pauseTask).Message.Should().Be(ErrorCodes.TaskFinnishedOrCancelled.ToString());
        }

        [Fact]
        public void PauseTask_TaskDoesNotHaveAllocatedUser_ThrowsDomainException()
        {
            // Arrange
            var keyValue = new ImmutableKeyValue(string.Empty, string.Empty);
            var processId = new ProcessId(keyValue);
            var taskId = new TaskId();
            var sut = Setup.GenerateTaskAllocationAggregate(new TaskDefinition("TaskDefinition004", Setup.bad, Setup.bad, Setup.good, null, null, null, null, null, new AutomaticStart(true)), taskId, processId);

            //Act
            var pauseTask = new Action(() => sut.Pause(processId.ToString()));

            // Assert
            Assert.Throws<DomainException>(pauseTask).Message.Should().Be(ErrorCodes.TaskHasNoAllocatedUser.ToString());
        }

        [Fact]
        public void PauseTask_TaskHaveUserIdAndIsAllocated_StateOfTaskBecomePaused()
        {
            // Arrange
            var keyValue = new ImmutableKeyValue(string.Empty, string.Empty);
            var processId = new ProcessId(keyValue);
            var taskId = new TaskId();
            var sut = Setup.GenerateTaskAllocationAggregate(new TaskDefinition("TaskDefinition004", Setup.bad, Setup.bad, Setup.good, null, null, null, null, null, new AutomaticStart(true)), taskId, processId);
            sut.AllocateUser(processId.ToString(), 1);

            //Act
            sut.Pause(processId.ToString());

            // Assert
            Assert.Equal(sut.State, TaskAllocationStatus.InStandby);
            var emittedEvents = sut.GetUncommittedChanges().ToList();
            emittedEvents.Where(e => e is TaskPaused).Should().HaveCount(1);
        }
    }
}