using FluentAssertions;
using System;
using Tasks.Runtime.Domain.ProcessObserverAggregate;
using Xunit;
using static Tasks.RuntimeDomain.Tests.ProcessObserverAggregate.Task_Test.Setup;

namespace Tasks.RuntimeDomain.Tests.ProcessObserverAggregate.Task_Test
{
    public class CreateTaskTests
    {
        [Fact]
        public void New_task_when_was_created_isInitiated()
        {
            //arrange
            var sut = CreateTaskWithNullExpressions();

            //act

            //assert
            sut.State.Should().Be(TaskState.Initiated);
        }

        [Fact]
        public void New_task_with_null_task_id_should_throw_exception()
        {
            //arrange

            //act
            var createTaskWithNullTaskDefinition = new Func<Task>(() => new Task(null, null));

            //assert
            Assert.Throws<ArgumentNullException>(createTaskWithNullTaskDefinition).ParamName.Should().Be("taskId");
        }

        [Fact]
        public void New_task_with_null_task_definition_should_throw_exception()
        {
            //arrange

            //act
            var createTaskWithNullTaskDefinition = new Func<Task>(() => new Task(new TaskId(), null));

            //assert
            Assert.Throws<ArgumentNullException>(createTaskWithNullTaskDefinition).ParamName.Should().Be("taskDefinition");
        }
    }
}
