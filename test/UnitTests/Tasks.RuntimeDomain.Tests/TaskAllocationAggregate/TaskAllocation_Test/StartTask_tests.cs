using FluentAssertions;
using NBB.Domain;
using System;
using System.Linq;
using Tasks.Runtime.Domain.Constants;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessObserverAggregate;
using Tasks.Runtime.Domain.TaskAllocationAggregate;
using Tasks.Runtime.Domain.TaskAllocationAggregate.DomainEvents;
using Xunit;
using static Tasks.RuntimeDomain.Tests.TaskAllocationAggregate.TaskAllocation_Test.Setup;

namespace Tasks.RuntimeDomain.Tests.TaskAllocationAggregate.TaskAllocation_Test
{
    public class StartTask_tests
    {
        [Fact]
        public void StartTask_TaskIsInProgress_ThrowsDomainException()
        {
            // Arrange
            var keyValue = new ImmutableKeyValue(string.Empty, string.Empty);
            var processId = new ProcessId(keyValue);
            var taskId = new TaskId();
            var sut = GenerateTaskAllocationAggregate(new TaskDefinition("TaskDefinition004", bad, bad, good, null, null, null, null, null, new AutomaticStart(false)), taskId, processId);
            sut.AllocateUser(processId.ToString(), 1);
            sut.Start(processId.ToString());//put the task in InProgress state

            //Act
            var startTask = new Action(() => sut.Start(processId.ToString()));

            // Assert
            Assert.Throws<DomainException>(startTask).Message.Should().Be(ErrorCodes.TaskAlreadyStarted.ToString());
        }

        [Fact]
        public void StartTask_TaskDoesNotHaveAllocatedUserId_ThrowsDomainException()
        {
            // Arrange
            var keyValue = new ImmutableKeyValue(string.Empty, string.Empty);
            var processId = new ProcessId(keyValue);
            var taskId = new TaskId();
            var sut = GenerateTaskAllocationAggregate(new TaskDefinition("TaskDefinition004", bad, bad, good, null, null, null, null, null, new AutomaticStart(true)), taskId, processId);

            //Act
            var startTask = new Action(() => sut.Start(processId.ToString()));

            // Assert
            Assert.Throws<DomainException>(startTask).Message.Should().Be(ErrorCodes.TaskHasNoAllocatedUser.ToString());
        }

        [Fact]
        public void StartTask_TaskIsFinished_ThrowsDomainException()
        {
            // Arrange
            var keyValue = new ImmutableKeyValue(string.Empty, string.Empty);
            var processId = new ProcessId(keyValue);
            var taskId = new TaskId();
            var sut = GenerateTaskAllocationAggregate(new TaskDefinition("TaskDefinition004", bad, bad, good, null, null, null, null, null, new AutomaticStart(true)), taskId, processId);
            sut.AllocateUser(processId.ToString(), 1);
            sut.Finish(processId);//put the task in Finished state

            //Act
            var startTask = new Action(() => sut.Start(processId.ToString()));

            // Assert
            Assert.Throws<DomainException>(startTask).Message.Should().Be(ErrorCodes.TaskFinnishedOrCancelled.ToString());
        }

        [Fact]
        public void StartTask_TaskIsCancelled_ThrowsDomainException()
        {
            // Arrange
            var keyValue = new ImmutableKeyValue(string.Empty, string.Empty);
            var processId = new ProcessId(keyValue);
            var taskId = new TaskId();
            var sut = GenerateTaskAllocationAggregate(new TaskDefinition("TaskDefinition004", bad, bad, good, null, null, null, null, null, new AutomaticStart(true)), taskId, processId);
            sut.AllocateUser(processId.ToString(), 1);
            sut.Cancel(processId);//put the task in Cancelled state

            //Act
            var startTask = new Action(() => sut.Start(processId.ToString()));

            // Assert
            Assert.Throws<DomainException>(startTask).Message.Should().Be(ErrorCodes.TaskFinnishedOrCancelled.ToString());
        }

        [Fact]
        public void StartTask_TaskHaveUserIdAndIsAllocated_StateOfTaskBecomeInProgress()
        {
            // Arrange
            var keyValue = new ImmutableKeyValue(string.Empty, string.Empty);
            var processId = new ProcessId(keyValue);
            var taskId = new TaskId();
            var sut = GenerateTaskAllocationAggregate(new TaskDefinition("TaskDefinition004", bad, bad, good, null, null, null, null, null, new AutomaticStart(false)), taskId, processId);
            sut.AllocateUser(processId.ToString(), 1);

            //Act
            sut.Start(processId.ToString());

            // Assert
            Assert.Equal(1, sut.UserId);
            Assert.Equal(sut.State, TaskAllocationStatus.InProgress);
            var emittedEvents = sut.GetUncommittedChanges().ToList();
            emittedEvents.Where(e => e is TaskStarted).Should().HaveCount(1);
        }
    }
}