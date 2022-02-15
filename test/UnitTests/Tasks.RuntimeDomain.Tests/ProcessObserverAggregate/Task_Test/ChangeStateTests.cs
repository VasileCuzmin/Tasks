using FluentAssertions;
using System.Collections.Generic;
using Tasks.Runtime.Domain.ProcessObserverAggregate;
using Xunit;
using static Tasks.RuntimeDomain.Tests.ProcessObserverAggregate.Task_Test.Setup;

namespace Tasks.RuntimeDomain.Tests.ProcessObserverAggregate.Task_Test
{
    public class ChangeStateTests
    {
        [Theory]
        [MemberData(nameof(GetTaskStates))]
        public void ChangeState_ChangesState_ReturnsNewState(TaskState state)
        {
            //arrange
            var sut = CreateTaskWithNullExpressions();

            //act
            sut.ChangeState(state);

            //assert
            sut.State.Should().Be(state);
        }
        public static IEnumerable<object[]> GetTaskStates()
        {
            return new List<object[]>
            {
                new object[] { TaskState.Initiated },
                new object[] { TaskState.Finished },
                new object[] { TaskState.Initiated },
                new object[] { TaskState.Canceled }
            };
        }
    }
}
