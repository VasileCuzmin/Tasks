using FluentAssertions;
using Moq;
using System;
using System.Collections.Immutable;
using Tasks.Runtime.Domain.EventDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessObserverAggregate;
using Tasks.Runtime.Domain.Services;
using Xunit;
using static Tasks.RuntimeDomain.Tests.ProcessObserverAggregate.Task_Test.Setup;

namespace Tasks.RuntimeDomain.Tests.ProcessObserverAggregate.Task_Test
{
    public class InitiatedTaskTests
    {
        private readonly Mock<IExpressionEvaluationService> _expressionEvaluationServiceMock;

        public InitiatedTaskTests()
        {
            _expressionEvaluationServiceMock = new Mock<IExpressionEvaluationService>();
        }

        [Fact]
        public void Initiated_task_can_change_state_with_close_event_when_expression_evaluates_to_true()
        {
            //arrange
            var sut = CreateTaskWithExpressions();
            var closeEventDefinitionId = sut.TaskDefinition.CloseEvent;
            var closeEvent = new DynamicEvent(closeEventDefinitionId, ImmutableDictionary.Create<string, object>(), Guid.Empty, string.Empty);
            var closeExpression = sut.TaskDefinition.CloseExpression;
            _expressionEvaluationServiceMock
                .Setup(x => x.EvaluateEventExpression(closeEvent, closeExpression)).Returns(true);

            //act
            var (willChange, newState) = sut.EventChangesState(closeEvent, _expressionEvaluationServiceMock.Object);

            //assert
            willChange.Should().BeTrue();
            newState.Should().Be(TaskState.Finished);
            sut.State.Should().Be(TaskState.Initiated);
            _expressionEvaluationServiceMock.Verify(
                x => x.EvaluateEventExpression(closeEvent, closeExpression), Times.Once);
        }

        [Fact]
        public void Initiated_task_cannot_change_state_with_close_event_when_expression_evaluates_to_false()
        {
            //arrange
            var sut = CreateTaskWithExpressions();
            var closeEventDefinitionId = sut.TaskDefinition.CloseEvent;
            var closeEvent = new DynamicEvent(closeEventDefinitionId, ImmutableDictionary.Create<string, object>(), Guid.Empty, string.Empty);
            var closeExpression = sut.TaskDefinition.CloseExpression;

            //act
            var (willChange, newState) = sut.EventChangesState(closeEvent, _expressionEvaluationServiceMock.Object);

            //assert
            willChange.Should().BeFalse();
            newState.Should().BeNull();
            sut.State.Should().Be(TaskState.Initiated);
            _expressionEvaluationServiceMock.Verify(
                x => x.EvaluateEventExpression(closeEvent, closeExpression), Times.Once);
        }

        [Fact]
        public void Initiated_task_shows_you_can_change_it_with_cancel_event_when_expression_evaluates_to_true()
        {
            //arrange
            var sut = CreateTaskWithExpressions();
            var cancelEventDefinitionId = sut.TaskDefinition.CancelEvent;
            var cancelEvent = new DynamicEvent(cancelEventDefinitionId, ImmutableDictionary.Create<string, object>(), Guid.Empty, string.Empty);
            var cancelExpression = sut.TaskDefinition.CancelExpression;
            _expressionEvaluationServiceMock
                .Setup(x => x.EvaluateEventExpression(cancelEvent, cancelExpression)).Returns(true);

            //act
            var (willChange, newState) = sut.EventChangesState(cancelEvent, _expressionEvaluationServiceMock.Object);

            //assert
            willChange.Should().BeTrue();
            newState.Should().Be(TaskState.Canceled);
            sut.State.Should().Be(TaskState.Initiated);
            _expressionEvaluationServiceMock.Verify(
                x => x.EvaluateEventExpression(cancelEvent, cancelExpression), Times.Once);
        }

        [Fact]
        public void Initiated_task_shows_you_cannot_change_it_with_cancel_event_when_expression_evaluates_to_false()
        {
            //arrange
            var sut = CreateTaskWithExpressions();
            var cancelEventDefinitionId = sut.TaskDefinition.CancelEvent;
            var cancelEvent = new DynamicEvent(cancelEventDefinitionId, ImmutableDictionary.Create<string, object>(), Guid.Empty, string.Empty);
            var cancelExpression = sut.TaskDefinition.CancelExpression;

            //act
            var (willChange, newState) = sut.EventChangesState(cancelEvent, _expressionEvaluationServiceMock.Object);

            //assert
            willChange.Should().BeFalse();
            newState.Should().BeNull();
            sut.State.Should().Be(TaskState.Initiated);
            _expressionEvaluationServiceMock.Verify(
                x => x.EvaluateEventExpression(cancelEvent, cancelExpression), Times.Once);
        }

        [Fact]
        public void Initiated_task_shows_you_cannot_change_it_with_start_event()
        {
            //arrange
            var sut = CreateTaskWithNullExpressions();
            var startEventDefinitionId = sut.TaskDefinition.StartEvent;
            var startEvent = new DynamicEvent(startEventDefinitionId, ImmutableDictionary.Create<string, object>(), Guid.Empty, string.Empty);

            //act
            var (willChange, newState) = sut.EventChangesState(startEvent, _expressionEvaluationServiceMock.Object);

            //assert
            willChange.Should().BeFalse();
            newState.Should().BeNull();
        }

        [Fact]
        public void Initiated_task_shows_you_cannot_change_it_with_unrelated_event_when_expression_evaluates_to_true()
        {
            //arrange
            var sut = CreateTaskWithExpressions();
            var unrelated = new DynamicEvent(new EventDefinitionName(Guid.NewGuid().ToString()), ImmutableDictionary.Create<string, object>(), Guid.Empty, string.Empty);
            var cancelExpression = sut.TaskDefinition.CancelExpression;
            _expressionEvaluationServiceMock
               .Setup(x => x.EvaluateEventExpression(unrelated, cancelExpression)).Returns(true);

            //act
            var (willChange, newState) = sut.EventChangesState(unrelated, _expressionEvaluationServiceMock.Object);

            //assert
            willChange.Should().BeFalse();
            newState.Should().BeNull();
        }
    }
}
